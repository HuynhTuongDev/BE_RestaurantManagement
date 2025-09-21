using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class MenuItemController : ControllerBase
    {
        private readonly IMenuItemImageService _menuItemImageService;
        private readonly IMenuItemService _menuItemServices;
        public MenuItemController(IMenuItemImageService menuItemImageService, IMenuItemService menuItemService)
        {
            _menuItemImageService = menuItemImageService;
            _menuItemServices = menuItemService;
        }

        // Search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var dishes = await _menuItemServices.SearchAsync(keyword ?? "");
            return Ok(dishes);
        }

        // Add
        [HttpPost]
        public async Task<IActionResult> AddDish(
            string name,
            string Description,
            decimal price,
            string Category
            )
        {
            var dish = new MenuItem { Name = name,Description =Description, Price = price ,Category=Category};
            var created = await _menuItemServices.AddAsync(dish);

            return CreatedAtAction(nameof(GetDish), new { id = created.Id }, created);
        }

        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDish(int id)
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) return NotFound();
            return Ok(dish);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> EditDish(
            int id,
            string name,
            string Description,
            decimal price,
            string Category
            )
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) return NotFound();

            dish.Name = name;
            dish.Description = Description;
            dish.Price = price;
            dish.Category = Category;
            
            


            await _menuItemServices.UpdateAsync(dish);
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) return NotFound();

            await _menuItemServices.DeleteAsync(id);
            return NoContent();
        }
    }

}
