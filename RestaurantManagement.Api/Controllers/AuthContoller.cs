using BackEnd.Service.ServiceImpl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Interfaces;
using System.Security.Claims;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthController(IAuthService authService, IUserRepository userRepository, IJwtService jwtService, IEmailService emailService)
        {
            _authService = authService;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);
            if (result.Success)
                return Ok(result);

            return Unauthorized(result);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GoogleLoginAsync(request);
            if (result.Success)
                return Ok(result);

            return Unauthorized(result);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token");

            var user = await _authService.GetUserProfileAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token");

            var result = await _authService.UpdateProfileAsync(userId, request);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token");

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }



        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userRepository.GetByEmailAsync(forgotRequest.Email);
            if (user == null)
            {
                return NotFound("User not found with the provided email.");
            }

            await _emailService.SendResetPasswordEmail(user);
            return Ok("Please check your email to reset your password.");
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claimsPrincipal = _jwtService.ValidateToken(resetRequest.Token!, "Reset");
            if (claimsPrincipal == null)
            {
                return BadRequest("The token is invalid or has expired.");
            }

            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

            var result = await _authService.UpdatePasswordAsync(email!, resetRequest);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
