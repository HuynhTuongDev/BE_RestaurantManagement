using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/menu-item")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Staff")]
    public class MenuItemController : BaseController
    {
        private readonly IMenuItemService _menuItemServices;
        
        public MenuItemController(IMenuItemService menuItemService, ILogger<MenuItemController> logger)
            : base(logger)
        {
            _menuItemServices = menuItemService;
        }

        /// <summary>
        /// Get all menu items
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Allow guest to view menu
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var items = await _menuItemServices.GetAllAsync();
            return OkListResponse(items, "Menu items retrieved successfully");
        }

        /// <summary>
        /// Get paginated menu items
        /// </summary>
        [HttpGet("paginated")]
        [AllowAnonymous] // Allow guest to view menu
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginated([FromQuery] PaginationRequest pagination)
        {
            var paginatedItems = await _menuItemServices.GetPaginatedAsync(pagination);
            return OkPaginatedResponse(paginatedItems, "Menu items retrieved successfully");
        }

        /// <summary>
        /// Search menu items by keyword
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var items = await _menuItemServices.SearchAsync(keyword.Trim());
            return OkListResponse(items, "Search completed successfully");
        }

        /// <summary>
        /// Search menu items with pagination
        /// </summary>
        [HttpGet("search/paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPaginated(
            [FromQuery] string keyword,
            [FromQuery] PaginationRequest pagination)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var paginatedItems = await _menuItemServices.SearchPaginatedAsync(keyword.Trim(), pagination);
            return OkPaginatedResponse(paginatedItems, "Search completed successfully");
        }

        /// <summary>
        /// Get menu item by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow guest to view menu item details
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDish(int id)
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) 
                return NotFoundResponse($"MenuItem {id} not found");
            
            return OkResponse(dish, "Menu item retrieved successfully");
        }

        /// <summary>
        /// Add a new menu item
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddDish([FromBody] MenuItemCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid menu item data");

            var created = await _menuItemServices.AddAsync(request);
            return CreatedResponse(nameof(GetDish), created.Id, created, "Menu item created successfully");
        }

        /// <summary>
        /// Update menu item
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EditDish(int id, [FromBody] MenuItemCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid menu item data");

            try
            {
                var updated = await _menuItemServices.UpdateAsync(id, request);
                return OkResponse(updated, "Menu item updated successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse($"MenuItem {id} not found");
            }
        }

        /// <summary>
        /// Delete menu item
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDish(int id)
        {
            try
            {
                await _menuItemServices.DeleteAsync(id);
                return OkResponse(new { deleted = true }, "Menu item deleted successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFoundResponse($"MenuItem {id} not found");
            }
        }
    }
}
