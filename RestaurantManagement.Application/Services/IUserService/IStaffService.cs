using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Application.Services.IUserService
{
    public interface IStaffService
    {
        Task<StaffResponse> CreateStaffAsync(StaffCreateRequest request);
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
        Task<StaffDto?> GetStaffByIdAsync(int id);
        Task<StaffResponse> UpdateStaffAsync(int id, StaffCreateRequest request);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<StaffDto>> SearchStaffAsync(string keyword);
    }
}
