using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestaurantManagement.Application.Settings;

namespace RestaurantManagement.Application.Services.System
{
    /// <summary>
    /// Image service implementation for Cloudinary
    /// </summary>
    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;
        private readonly ILogger<ImageService> _logger;

        public ImageService(IOptions<CloudinarySettings> options, ILogger<ImageService> logger)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger;

            if (string.IsNullOrWhiteSpace(_settings.CloudName) || 
                string.IsNullOrWhiteSpace(_settings.ApiKey) || 
                string.IsNullOrWhiteSpace(_settings.ApiSecret))
                throw new ArgumentException("Cloudinary configuration is missing");

            var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

        /// <summary>
        /// Upload image to Cloudinary
        /// </summary>
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            if (fileStream == null || fileStream.Length == 0)
                throw new ArgumentException("File stream is empty", nameof(fileStream));

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_settings.AllowedFormats.Contains(ext))
                throw new ArgumentException("File format not supported", nameof(fileName));

            if (_settings.MaxFileSizeKb > 0 && fileStream.Length > _settings.MaxFileSizeKb * 1024)
                throw new ArgumentException($"File size exceeds the maximum allowed size of {_settings.MaxFileSizeKb} KB", nameof(fileStream));

            // Reset stream position if possible
            if (fileStream.CanSeek)
                fileStream.Position = 0;

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, fileStream),
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
                    .Crop("limit")
                    .Width(1920)
                    .Height(1080)
                    .Dpr("auto"),
                Folder = _settings.Folder,
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);
            if (uploadResult == null)
                throw new InvalidOperationException("Upload failed, no result returned");

            if (uploadResult.Error != null)
                throw new InvalidOperationException($"Cloudinary upload error: {uploadResult.Error.Message}");

            return uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString() ?? string.Empty;
        }

        /// <summary>
        /// Delete image from Cloudinary
        /// </summary>
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(publicId))
                    return false;

                var deleteParams = new DeletionParams(publicId);
                var result = await _cloudinary.DestroyAsync(deleteParams);

                _logger.LogInformation("Image deleted: {PublicId}, Result: {Result}", publicId, result.Result);

                return result.Result == "ok";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image: {PublicId}", publicId);
                return false;
            }
        }

        /// <summary>
        /// Get image URL by public ID
        /// </summary>
        public string GetImageUrl(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                return string.Empty;

            return _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
        }

        /// <summary>
        /// Validate image file
        /// </summary>
        public bool ValidateImage(string fileName, long fileSize)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            if (!_settings.AllowedFormats.Contains(ext))
            {
                _logger.LogWarning("Invalid file format: {Extension}", ext);
                return false;
            }

            if (_settings.MaxFileSizeKb > 0 && fileSize > _settings.MaxFileSizeKb * 1024)
            {
                _logger.LogWarning("File size {Size} exceeds maximum {Max}", fileSize, _settings.MaxFileSizeKb * 1024);
                return false;
            }

            return true;
        }
    }
}