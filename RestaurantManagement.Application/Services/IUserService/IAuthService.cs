using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Application.Services.IUserService
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> GoogleLoginAsync(GoogleLoginRequest request);
        Task<AuthResponse> UpdateProfileAsync(int userId, UpdateProfileRequest request);
        Task<AuthResponse> ChangePasswordAsync(int userId, ChangePasswordRequest request);
        Task<UserDto?> GetUserProfileAsync(int userId);
        Task<AuthResponse> UpdatePasswordAsync(string email, ResetPasswordRequest request);
    }
}
