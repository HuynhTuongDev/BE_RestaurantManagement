using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Infrastructure.Services.UserServices
{
    public class StaffService : IStaffService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<StaffService> _logger;

        public StaffService(IUserRepository userRepository, ILogger<StaffService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<StaffResponse> CreateStaffAsync(StaffCreateRequest request)
        {
            try
            {
                // Check if email already exists
                if (!string.IsNullOrEmpty(request.Email) && await _userRepository.EmailExistsAsync(request.Email))
                {
                    return new StaffResponse { Success = false, Message = "Email already exists" };
                }

                var user = new User
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    Phone = request.Phone,
                    Address = request.Address,
                    Role = UserRole.Staff,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);

                // Create StaffProfile separately after user is created
                var staffProfile = new StaffProfile
                {
                    UserId = user.Id,
                    Position = request.Position,
                    HireDate = request.HireDate
                };

                user.StaffProfile = staffProfile;
                await _userRepository.UpdateAsync(user);

                return new StaffResponse
                {
                    Success = true,
                    Message = "Staff created successfully",
                    Staff = MapToStaffDto(user, staffProfile)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff");
                return new StaffResponse { Success = false, Message = "An error occurred while creating staff" };
            }
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            try
            {
                var staffUsers = await _userRepository.GetByRoleAsync(UserRole.Staff);
                return staffUsers.Where(u => u.StaffProfile != null)
                               .Select(u => MapToStaffDto(u, u.StaffProfile!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all staff");
                return new List<StaffDto>();
            }
        }

        public async Task<StaffDto?> GetStaffByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdWithProfileAsync(id);
                if (user?.Role == UserRole.Staff && user.StaffProfile != null)
                {
                    return MapToStaffDto(user, user.StaffProfile);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting staff {Id}", id);
                return null;
            }
        }

        public async Task<StaffResponse> UpdateStaffAsync(int id, StaffCreateRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdWithProfileAsync(id);
                if (user?.Role != UserRole.Staff)
                {
                    return new StaffResponse { Success = false, Message = "Staff not found" };
                }

                // Check email exists for other users
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email && 
                    await _userRepository.EmailExistsAsync(request.Email))
                {
                    return new StaffResponse { Success = false, Message = "Email already exists" };
                }

                user.FullName = request.FullName;
                user.Email = request.Email;
                user.Phone = request.Phone;
                user.Address = request.Address;
                user.UpdatedAt = DateTime.UtcNow;

                if (user.StaffProfile != null)
                {
                    user.StaffProfile.Position = request.Position;
                    user.StaffProfile.HireDate = request.HireDate;
                }

                await _userRepository.UpdateAsync(user);

                return new StaffResponse
                {
                    Success = true,
                    Message = "Staff updated successfully",
                    Staff = MapToStaffDto(user, user.StaffProfile!)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating staff {Id}", id);
                return new StaffResponse { Success = false, Message = "An error occurred while updating staff" };
            }
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user?.Role == UserRole.Staff)
                {
                    return await _userRepository.SoftDeleteUserAsync(id);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting staff {Id}", id);
                return false;
            }
        }

        public async Task<IEnumerable<StaffDto>> SearchStaffAsync(string keyword)
        {
            try
            {
                var staffUsers = await _userRepository.SearchByKeywordAsync(keyword, UserRole.Staff);
                return staffUsers.Where(u => u.StaffProfile != null)
                               .Select(u => MapToStaffDto(u, u.StaffProfile!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching staff with keyword {Keyword}", keyword);
                return new List<StaffDto>();
            }
        }

        private static StaffDto MapToStaffDto(User user, StaffProfile staffProfile)
        {
            return new StaffDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted,
                Position = staffProfile.Position,
                HireDate = staffProfile.HireDate
            };
        }
    }
}
