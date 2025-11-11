using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/menuitems/{menuItemId:int}/images")]
    [ApiVersion("1.0")]
    public class MenuItemImagesController : BaseController
    {
        private readonly IMenuItemImageService _menuItemImageService;

        public MenuItemImagesController(IMenuItemImageService menuItemImageService, ILogger<MenuItemImagesController> logger)
            : base(logger)
        {
            _menuItemImageService = menuItemImageService;
        }

        /// <summary>
        /// Get all images for a menu item
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImages(int menuItemId)
        {
            try
            {
                var images = await _menuItemImageService.GetImagesByMenuItemIdAsync(menuItemId);
                if (images == null || !images.Any())
                    return NotFoundResponse("No images found for this menu item");

                return OkListResponse(images, "Images retrieved successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse("MenuItem not found");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving images for menu item {MenuItemId}", menuItemId);
                return InternalServerErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get paginated images for a menu item
        /// </summary>
        [HttpGet("paginated")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaginatedImages(int menuItemId, [FromQuery] PaginationRequest pagination)
        {
            try
            {
                var paginatedImages = await _menuItemImageService.GetPaginatedImagesByMenuItemIdAsync(menuItemId, pagination);
                return OkPaginatedResponse(paginatedImages, "Images retrieved successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse("MenuItem not found");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving paginated images for menu item {MenuItemId}", menuItemId);
                return InternalServerErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Get a specific image by ID
        /// </summary>
        [HttpGet("{imageId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImage(int menuItemId, int imageId)
        {
            try
            {
                var image = await _menuItemImageService.GetImageByIdAsync(imageId);
                if (image == null || image.MenuItemId != menuItemId)
                    return NotFoundResponse("Image not found for this menu item");

                return OkResponse(image, "Image retrieved successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving image {ImageId}", imageId);
                return InternalServerErrorResponse(ex.Message);
            }
        }

        /// <summary>
        /// Upload an image for a menu item
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Upload(int menuItemId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequestResponse("No file provided");

            try
            {
                await using var stream = file.OpenReadStream();
                var menuImage = await _menuItemImageService.UploadMenuItemImageAsync(menuItemId, stream, file.FileName, cancellationToken);

                return CreatedResponse(
                    nameof(GetImage),
                    menuImage.Id,
                    new { id = menuImage.Id, menuItemId = menuImage.MenuItemId, url = menuImage.ImageUrl },
                    "Image uploaded successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse("MenuItem not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequestResponse(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return InternalServerErrorResponse(ex.Message);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error uploading image for menu item {MenuItemId}", menuItemId);
                return InternalServerErrorResponse("An unexpected error occurred");
            }
        }

        /// <summary>
        /// Delete an image
        /// </summary>
        [HttpDelete("{imageId:int}")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(int menuItemId, int imageId)
        {
            try
            {
                var success = await _menuItemImageService.DeleteImageAsync(imageId);
                if (!success)
                    return NotFoundResponse("Image not found");

                return OkResponse(new { deleted = true }, "Image deleted successfully");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting image {ImageId}", imageId);
                return InternalServerErrorResponse(ex.Message);
            }
        }
    }
}
