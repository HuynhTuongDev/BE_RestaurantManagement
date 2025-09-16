using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/menuitems/{menuItemId:int}/images")]
    [Authorize(Roles = "Admin,Staff")] // Only admin and staff can upload images
    public class MenuItemImagesController : ControllerBase
    {
        private readonly IMenuItemImageService _menuItemImageService;

        public MenuItemImagesController(IMenuItemImageService menuItemImageService)
        {
            _menuItemImageService = menuItemImageService;
        }

        /// <summary>
        /// Upload an image for a menu item
        /// </summary>
        /// <param name="menuItemId">Menu item ID</param>
        /// <param name="file">Image file</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Created menu item image</returns>
        [HttpPost]
        public async Task<IActionResult> Upload(int menuItemId, IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            try
            {
                await using var stream = file.OpenReadStream();
                var menuImage = await _menuItemImageService.UploadMenuItemImageAsync(menuItemId, stream, file.FileName, cancellationToken);

                return CreatedAtAction(
                    nameof(GetImage), 
                    new { menuItemId, imageId = menuImage.Id }, 
                    new { 
                        id = menuImage.Id, 
                        menuItemId = menuImage.MenuItemId,
                        url = menuImage.ImageUrl 
                    });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "MenuItem not found" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", detail = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific image by ID
        /// </summary>
        /// <param name="menuItemId">Menu item ID</param>
        /// <param name="imageId">Image ID</param>
        /// <returns>Menu item image</returns>
        [HttpGet("{imageId:int}")]
        [AllowAnonymous] // Allow public access to view images
        public async Task<IActionResult> GetImage(int menuItemId, int imageId)
        {
            // This endpoint can be implemented later when needed
            await Task.CompletedTask; // Fixes CS1998 by adding an await
            return NotFound(new { message = "Endpoint not implemented" });
        }
    }
}
