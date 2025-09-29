using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;

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

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] int TableNumber) =>
        Ok(await _restaurantTableService.SearchAsync(TableNumber));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] RestaurantTableCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var created = await _restaurantTableService.AddAsync(dto);
            return CreatedAtAction(nameof(GetTable), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] RestaurantTableCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _restaurantTableService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _restaurantTableService.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("{id}/reserve")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Reserve(int id) =>
            await _restaurantTableService.ReserveAsync(id) ? Ok() : NotFound();

        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> Cancel(int id) =>
            await _restaurantTableService.CancelReservationAsync(id) ? Ok() : NotFound();

    }
}

