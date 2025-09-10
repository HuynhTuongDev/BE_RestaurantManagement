using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IJwtService jwtService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                return new AuthResponse { Success = false, Message = "Email already exists" };
            }

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Registration successful",
                Token = token,
                User = MapToUserDto(user, user.FullName)
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new AuthResponse { Success = false, Message = "Invalid email or password" };
            }

            await _userRepository.UpdateAsync(user);

            var token = _jwtService.GenerateToken(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = MapToUserDto(user)
            };
        }

        public async Task<AuthResponse> GoogleLoginAsync(GoogleLoginRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    request.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new[] { _configuration["Google:ClientId"] }
                    });

                var user = await _userRepository.GetByEmailAsync(payload.Email!);
                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email!,
                        PasswordHash = Guid.NewGuid().ToString(),
                        FullName = $"{payload.GivenName ?? ""} {payload.FamilyName ?? ""}".Trim(),
                        Role = UserRole.Customer,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _userRepository.AddAsync(user);
                }

                await _userRepository.UpdateAsync(user);

                var token = _jwtService.GenerateToken(user);
                return new AuthResponse
                {
                    Success = true,
                    Message = "Google login successful",
                    Token = token,
                    User = MapToUserDto(user)
                };
            }
            catch (InvalidJwtException)
            {
                return new AuthResponse { Success = false, Message = "Invalid Google token" };
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, Message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<AuthResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not found" };

            user.FullName = request.FullName;
            await _userRepository.UpdateAsync(user);

            return new AuthResponse
            {
                Success = true,
                Message = "Update successful",
                User = MapToUserDto(user, request.FullName)
            };
        }

        public async Task<AuthResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return new AuthResponse { Success = false, Message = "User not found" };

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return new AuthResponse { Success = false, Message = "Current password is incorrect" };

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            return new AuthResponse { Success = true, Message = "Password changed successfully" };
        }

        public async Task<UserDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null ? MapToUserDto(user) : null;
        }

        private static UserDto MapToUserDto(User user, string? fullName = null)
        {
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email
            };
        }
    }
}
