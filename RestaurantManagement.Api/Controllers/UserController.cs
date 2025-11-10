using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/user")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin")]
    public class UserController : BaseController
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository, ILogger<UserController> logger)
            : base(logger)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Soft delete user
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userRepository.SoftDeleteUserAsync(id);

            if (result)
                return OkResponse(new { deleted = true }, "User deleted (soft) successfully");

            return NotFoundResponse("User not found");
        }

        /// <summary>
        /// Lock/suspend user
        /// </summary>
        [HttpPut("{id}/lock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LockUser(int id)
        {
            var result = await _userRepository.LockUserAsync(id);

            if (result)
                return OkResponse(new { locked = true }, "User locked successfully");

            return NotFoundResponse("User not found");
        }
    }
}
