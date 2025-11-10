using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Application.Services.System;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Interfaces;
using System.Security.Claims;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/auth")]
    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthController(
            IAuthService authService,
            IUserRepository userRepository,
            IJwtService jwtService,
            IEmailService emailService,
            ILogger<AuthController> logger)
            : base(logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _jwtService = jwtService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid registration data");

            var result = await _authService.RegisterAsync(request);

            if (result.Success)
                return OkResponse(result, "Registration successful");

            return BadRequestResponse(result.Message);
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid login data");

            var result = await _authService.LoginAsync(request);

            if (result.Success)
                return OkResponse(result, "Login successful");

            return UnauthorizedResponse(result.Message);
        }

        [HttpPost("google-login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid Google login data");

            var result = await _authService.GoogleLoginAsync(request);

            if (result.Success)
                return OkResponse(result, "Google login successful");

            return UnauthorizedResponse(result.Message);
        }

        [Authorize]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return UnauthorizedResponse("Invalid token");

            var user = await _authService.GetUserProfileAsync(userId);
            if (user == null)
                return NotFoundResponse("User not found");

            return OkResponse(user, "Profile retrieved successfully");
        }

        [Authorize]
        [HttpPut("profile")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid profile data");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return UnauthorizedResponse("Invalid token");

            var result = await _authService.UpdateProfileAsync(userId, request);

            if (result.Success)
                return OkResponse(result, "Profile updated successfully");

            return BadRequestResponse(result.Message);
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid password data");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return UnauthorizedResponse("Invalid token");

            var result = await _authService.ChangePasswordAsync(userId, request);

            if (result.Success)
                return OkResponse(result, "Password changed successfully");

            return BadRequestResponse(result.Message);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotRequest)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid email");

            var user = await _userRepository.GetByEmailAsync(forgotRequest.Email);
            if (user == null)
                return NotFoundResponse("User not found with the provided email");

            await _emailService.SendResetPasswordEmail(user);

            return OkResponse(new { message = "Please check your email to reset your password" },
                "Reset email sent successfully");
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetRequest)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid reset password data");

            var claimsPrincipal = _jwtService.ValidateToken(resetRequest.Token!, "Reset");
            if (claimsPrincipal == null)
                return BadRequestResponse("The token is invalid or has expired");

            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;

            var result = await _authService.UpdatePasswordAsync(email!, resetRequest);

            if (result.Success)
                return OkResponse(result, "Password reset successfully");

            return BadRequestResponse(result.Message);
        }
    }
}
