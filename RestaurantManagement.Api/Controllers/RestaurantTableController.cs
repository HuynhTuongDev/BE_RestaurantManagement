using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class RestaurantTableController : ControllerBase
    {
        private readonly IRestaurantTableService _restaurantTableService;
        public RestaurantTableController(IRestaurantTableService restaurantTableService)
        {
            _restaurantTableService = restaurantTableService;
        }
        
        // get list table by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTable(int id)
        {
            var table = await _restaurantTableService.GetByIdAsync(id);
            if (table == null) return NotFound();
            return Ok(table);
        }
        // get all table
        [HttpGet]
        public async Task<IActionResult> GetAllTables()
        {
            var tables = await _restaurantTableService.GetAllAsync();
            return Ok(tables);
        }
    }
}
