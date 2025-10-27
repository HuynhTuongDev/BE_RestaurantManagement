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

        // Search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var dishes = await _menuItemServices.SearchAsync(keyword ?? "");
            return Ok(dishes);
        }

        // Add
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


        [HttpGet("{id}")]
        public async Task<IActionResult> GetDish(int id)
        {
            var dish = await _menuItemServices.GetByIdAsync(id);
            if (dish == null) return NotFound();
            return Ok(dish);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> EditDish(int id,
            [FromBody] MenuItemCreateDto request)
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
