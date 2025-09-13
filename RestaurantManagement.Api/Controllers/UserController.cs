using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userRepository.SoftDeleteUserAsync(id);
            if (result)
                return Ok("User deleted (soft)");
            return NotFound("User not found");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("user/{id}/lock")]
        public async Task<IActionResult> LockUser(int id)
        {
            var result = await _userRepository.LockUserAsync(id);
            if (result)
                return Ok("User locked");
            return NotFound("User not found");
        }
    }
}
