using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/restaurant-table")]
    public class RestaurantTableController : ControllerBase
    {
        private readonly IRestaurantTableService _restaurantTableService;

        public RestaurantTableController(IRestaurantTableService restaurantTableService)
        {
            _restaurantTableService = restaurantTableService;
        }

        // --- Admin/Staff: Get table by id ---
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetTable(int id)
        {
            var table = await _restaurantTableService.GetByIdAsync(id);
            if (table == null) return NotFound();
            return Ok(table);
        }

        // --- Admin/Staff: Get all tables ---
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllTables()
        {
            var tables = await _restaurantTableService.GetAllAsync();
            return Ok(tables);
        }

        // --- Search table by number (Admin/Staff) ---
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Search([FromQuery] int TableNumber) =>
            Ok(await _restaurantTableService.SearchAsync(TableNumber));

        // --- Admin: Create table ---
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RestaurantTableCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _restaurantTableService.AddAsync(dto);
            return CreatedAtAction(nameof(GetTable), new { id = created.Id }, created);
        }

        // --- Admin: Update table ---
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RestaurantTableCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _restaurantTableService.UpdateAsync(id, dto);
            return NoContent();
        }

        // --- Admin: Delete table ---
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _restaurantTableService.DeleteAsync(id);
            return NoContent();
        }

        // --- Get all available tables (any logged-in user) ---
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableTables()
        {
            var tables = await _restaurantTableService.GetAllAsync();
            var available = tables.Where(t => t.Status == TableStatus.Available);
            return Ok(available);
        }

        // --- Reserve a table (any logged-in user) ---
        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> Reserve(int id)
        {
            var result = await _restaurantTableService.ReserveAsync(id);
            return result ? Ok() : BadRequest("Bàn không khả dụng hoặc đã được đặt");
        }

        // --- Cancel reservation (any logged-in user) ---
        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _restaurantTableService.CancelReservationAsync(id);
            return result ? Ok() : BadRequest("Không thể hủy bàn này");
        }
    }
}
