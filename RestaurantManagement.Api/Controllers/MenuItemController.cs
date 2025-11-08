using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/menu-item")]
    [Authorize(Roles = "Admin,Staff")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemService _menuItemServices;
        
        public MenuItemController(IMenuItemService menuItemService)
        {
            _menuItemServices = menuItemService;
        }

        /// <summary>
        /// Get all menu items
        /// </summary>
        [HttpGet]
        [AllowAnonymous] // Allow guest to view menu
        public async Task<IActionResult> GetAll()
        {
            var items = await _menuItemServices.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// Search menu items by keyword
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            var items = await _menuItemServices.SearchAsync(keyword.Trim());
            return Ok(items);
        }

        /// <summary>
        /// Get menu item by ID
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow guest to view menu item details
        public async Task<IActionResult> GetDish(int id)
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) 
                return NotFound(new { message = $"MenuItem {id} not found" });
            return Ok(dish);
        }

        /// <summary>
        /// Add a new menu item
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddDish([FromBody] MenuItemCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dish = new MenuItem
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Category = request.Category
            };

            var created = await _menuItemServices.AddAsync(dish);
            return CreatedAtAction(nameof(GetDish), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update menu item
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditDish(int id, [FromBody] MenuItemCreateDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null)
                return NotFound(new { message = $"MenuItem {id} not found" });

            dish.Name = request.Name;
            dish.Description = request.Description;
            dish.Price = request.Price;
            dish.Category = request.Category;

            await _menuItemServices.UpdateAsync(dish);
            return NoContent();
        }

        /// <summary>
        /// Delete menu item
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) 
                return NotFound(new { message = $"MenuItem {id} not found" });

            await _menuItemServices.DeleteAsync(id);
            return NoContent();
        }
    }
}
