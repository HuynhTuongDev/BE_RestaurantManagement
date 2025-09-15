using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;

namespace RestaurantManagement.Api.Controllers
{
    [Authorize(Roles = "Admin")] 
    [ApiController]
    [Route("api/staff")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        [HttpPost("add-staff")]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCreateRequest request)
        {
            try
            {
                var staff = await _staffService.CreateStaffAsync(request);
                return Ok(new { message = "Staff created successfully", staff });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
