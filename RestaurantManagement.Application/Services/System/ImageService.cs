using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using RestaurantManagement.Application.Settings;

namespace RestaurantManagement.Application.Services.System
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    }

    public class ImageService : IImageService
    {
        private readonly Cloudinary _cloudinary;
        private readonly CloudinarySettings _settings;

        public ImageService(IOptions<CloudinarySettings> options)
        {
            _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(_settings.CloudName) || string.IsNullOrWhiteSpace(_settings.ApiKey) || string.IsNullOrWhiteSpace(_settings.ApiSecret))
                throw new ArgumentException("Cloudinary configuration is missing");

            var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
            _cloudinary = new Cloudinary(account)
            {
                Api = { Secure = true }
            };
        }

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
    }
}