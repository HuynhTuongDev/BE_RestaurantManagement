using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/staff")]
    [Authorize(Roles = "Admin")] // Only Admin can manage staff
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Create a new staff member
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _staffService.CreateStaffAsync(request);

            if (result.Success)
                return CreatedAtAction(nameof(GetStaff), new { id = result.Staff!.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get all staff members
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();
            return Ok(staff);
        }

        /// <summary>
        /// Get staff member by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStaff(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);

            if (staff == null)
                return NotFound(new { message = "Staff not found" });

            return Ok(staff);
        }

        /// <summary>
        /// Update staff member
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _staffService.UpdateStaffAsync(id, request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Delete staff member (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);

            if (result)
                return Ok(new { message = "Staff deleted successfully" });

            return NotFound(new { message = "Staff not found" });
        }

        /// <summary>
        /// Search staff members
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchStaff([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new { message = "Keyword is required" });

            var staff = await _staffService.SearchStaffAsync(keyword);
            return Ok(staff);
        }
    }
}
