using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/restaurant-table")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin,Staff")]
    public class RestaurantTableController : BaseController
    {
        private readonly IRestaurantTableService _restaurantTableService;
        
        public RestaurantTableController(
            IRestaurantTableService restaurantTableService,
            ILogger<RestaurantTableController> logger)
            : base(logger)
        {
            _restaurantTableService = restaurantTableService;
        }

        /// <summary>
        /// Get table by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTable(int id)
        {
            var table = await _restaurantTableService.GetByIdAsync(id);
            if (table == null) 
                return NotFoundResponse("Table not found");
            
            return OkResponse(table, "Table retrieved successfully");
        }

        /// <summary>
        /// Get all tables
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTables()
        {
            var tables = await _restaurantTableService.GetAllAsync();
            return OkListResponse(tables, "Tables retrieved successfully");
        }

        /// <summary>
        /// Get paginated tables
        /// </summary>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginatedTables([FromQuery] PaginationRequest pagination)
        {
            var paginatedTables = await _restaurantTableService.GetPaginatedAsync(pagination);
            return OkPaginatedResponse(paginatedTables, "Tables retrieved successfully");
        }

        /// <summary>
        /// Search tables by table number
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] int TableNumber)
        {
            var tables = await _restaurantTableService.SearchAsync(TableNumber);
            return OkListResponse(tables, "Search completed successfully");
        }

        /// <summary>
        /// Search tables by table number with pagination
        /// </summary>
        [HttpGet("search/paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchPaginated(
            [FromQuery] int TableNumber,
            [FromQuery] PaginationRequest pagination)
        {
            var paginatedTables = await _restaurantTableService.SearchPaginatedAsync(TableNumber, pagination);
            return OkPaginatedResponse(paginatedTables, "Search completed successfully");
        }

        /// <summary>
        /// Create a new table
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RestaurantTableCreateDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequestResponse("Invalid table data");
            
            var created = await _restaurantTableService.AddAsync(dto);
            return CreatedResponse(nameof(GetTable), created.Id, created, "Table created successfully");
        }

        /// <summary>
        /// Update table
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] RestaurantTableCreateDto dto)
        {
            if (!ModelState.IsValid) 
                return BadRequestResponse("Invalid table data");
            
            await _restaurantTableService.UpdateAsync(id, dto);
            return OkResponse(new { updated = true }, "Table updated successfully");
        }

        /// <summary>
        /// Delete table
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int id)
        {
            await _restaurantTableService.DeleteAsync(id);
            return OkResponse(new { deleted = true }, "Table deleted successfully");
        }

        /// <summary>
        /// Reserve a table
        /// </summary>
        [HttpPost("{id}/reserve")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reserve(int id)
        {
            var success = await _restaurantTableService.ReserveAsync(id);
            
            if (success)
                return OkResponse(new { reserved = true }, "Table reserved successfully");
            
            return NotFoundResponse("Table not found or cannot be reserved");
        }

        /// <summary>
        /// Cancel table reservation
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Staff,Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _restaurantTableService.CancelReservationAsync(id);
            
            if (success)
                return OkResponse(new { cancelled = true }, "Reservation cancelled successfully");
            
            return NotFoundResponse("Table not found or reservation cannot be cancelled");
        }
    }
}

