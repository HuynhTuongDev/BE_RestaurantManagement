namespace RestaurantManagement.Application.Services.System;

/// <summary>
/// Image service interface for image upload/management
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Upload image to cloud storage
    /// </summary>
    /// <param name="imageStream">Image stream</param>
    /// <param name="fileName">File name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Image URL</returns>
    Task<string> UploadImageAsync(
        Stream imageStream,
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete image from cloud storage
    /// </summary>
    /// <param name="publicId">Public ID of image</param>
    /// <returns>True if deleted successfully</returns>
    Task<bool> DeleteImageAsync(string publicId);

    /// <summary>
    /// Get image URL by public ID
    /// </summary>
    /// <param name="publicId">Public ID of image</param>
    /// <returns>Image URL</returns>
    string GetImageUrl(string publicId);

    /// <summary>
    /// Validate image file
    /// </summary>
    /// <param name="fileName">File name</param>
    /// <param name="fileSize">File size in bytes</param>
    /// <returns>True if valid</returns>
    bool ValidateImage(string fileName, long fileSize);
}
