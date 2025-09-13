using Microsoft.Extensions.Logging;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Services
{
    public class MenuItemImageService : IMenuItemImageService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IMenuItemImageRepository _menuItemImageRepository;
        private readonly IImageService _imageService;
        private readonly ILogger<MenuItemImageService> _logger;
        private readonly Cloudinary _cloudinary;

        public MenuItemImageService(
            IMenuItemRepository menuItemRepository,
            IMenuItemImageRepository menuItemImageRepository,
            IImageService imageService,
            ILogger<MenuItemImageService> logger,
            Cloudinary cloudinary)
        {
            _menuItemRepository = menuItemRepository;
            _menuItemImageRepository = menuItemImageRepository;
            _imageService = imageService;
            _logger = logger;
            _cloudinary = cloudinary;
        }

        public async Task<MenuItemImage> UploadMenuItemImageAsync(int menuItemId, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
        {
            // Validate MenuItem exists
            var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
            if (menuItem == null)
                throw new KeyNotFoundException($"MenuItem with ID {menuItemId} not found");

            string imageUrl;
            try
            {
                // Upload image to Cloudinary
                imageUrl = await _imageService.UploadImageAsync(fileStream, fileName, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload image for MenuItem {MenuItemId}", menuItemId);
                throw new InvalidOperationException("Image upload failed", ex);
            }

            // Create MenuItemImage entity
            var menuImage = new MenuItemImage
            {
                MenuItemId = menuItemId,
                ImageUrl = imageUrl
            };

            try
            {
                // Save to database
                await _menuItemImageRepository.AddAsync(menuImage);
                _logger.LogInformation("Successfully uploaded image for MenuItem {MenuItemId}: {ImageUrl}", menuItemId, imageUrl);
                return menuImage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save MenuItemImage to database. Attempting cleanup for {ImageUrl}", imageUrl);
                
                // Cleanup uploaded image from Cloudinary
                await CleanupImageAsync(imageUrl);
                
                throw new InvalidOperationException("Failed to save image record", ex);
            }
        }

        private async Task CleanupImageAsync(string imageUrl)
        {
            try
            {
                var publicId = ExtractPublicIdFromUrl(imageUrl);
                if (!string.IsNullOrEmpty(publicId))
                {
                    var deleteParams = new DeletionParams(publicId);
                    var deleteResult = await _cloudinary.DestroyAsync(deleteParams);
                    
                    if (deleteResult?.Result == "ok")
                    {
                        _logger.LogInformation("Successfully cleaned up Cloudinary image: {PublicId}", publicId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to cleanup Cloudinary image: {PublicId}, Result: {Result}", publicId, deleteResult?.Result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Cloudinary cleanup for URL: {ImageUrl}", imageUrl);
            }
        }

        private static string? ExtractPublicIdFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                
                if (segments.Length == 0) return null;

                // Find the segment after 'upload' (Cloudinary URL structure)
                var uploadIndex = Array.FindIndex(segments, s => s.Equals("upload", StringComparison.OrdinalIgnoreCase));
                if (uploadIndex == -1 || uploadIndex >= segments.Length - 1) return null;

                // Skip version if present (v1234567890)
                var startIndex = uploadIndex + 1;
                if (startIndex < segments.Length && segments[startIndex].StartsWith("v") && 
                    segments[startIndex].Length > 1 && segments[startIndex].Substring(1).All(char.IsDigit))
                {
                    startIndex++;
                }

                if (startIndex >= segments.Length) return null;

                // Get remaining segments (folder + filename)
                var pathSegments = segments.Skip(startIndex);
                var fullPath = string.Join("/", pathSegments);
                
                // Remove file extension
                var dotIndex = fullPath.LastIndexOf('.');
                return dotIndex > 0 ? fullPath.Substring(0, dotIndex) : fullPath;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}