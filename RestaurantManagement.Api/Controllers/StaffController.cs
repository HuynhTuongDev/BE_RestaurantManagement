using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/staff")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")] // Only Admin can manage staff
    public class StaffController : BaseController
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService, ILogger<StaffController> logger)
            : base(logger)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// Create a new staff member
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StaffResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid staff data");

            var result = await _staffService.CreateStaffAsync(request);

            if (result.Success)
                return CreatedResponse(nameof(GetStaff), result.Staff!.Id, result, "Staff created successfully");

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Get all staff members
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _staffService.GetAllStaffAsync();
            return OkListResponse(staff, "Staff retrieved successfully");
        }

        /// <summary>
        /// Get staff member by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaff(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);

            if (staff == null)
                return NotFoundResponse("Staff not found");

            return OkResponse(staff, "Staff retrieved successfully");
        }

        /// <summary>
        /// Update staff member
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] StaffCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid staff data");

            var result = await _staffService.UpdateStaffAsync(id, request);

            if (result.Success)
                return OkResponse(result, "Staff updated successfully");

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Delete staff member (soft delete)
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var result = await _staffService.DeleteStaffAsync(id);

            if (result)
                return OkResponse(new { deleted = true }, "Staff deleted successfully");

            return NotFoundResponse("Staff not found");
        }

        /// <summary>
        /// Search staff members
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchStaff([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var staff = await _staffService.SearchStaffAsync(keyword);
            return OkListResponse(staff, "Search completed successfully");
        }
    }
}
