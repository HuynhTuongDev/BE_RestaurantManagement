using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Application.Services.IUserService
{
    public interface IStaffService
    {
        Task<(bool success, string message, StaffDto? staff)> CreateStaffAsync(StaffCreateRequest request);
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
        Task<PaginatedResponse<StaffDto>> GetPaginatedAsync(PaginationRequest pagination);
        Task<StaffDto?> GetStaffByIdAsync(int id);
        Task<(bool success, string message, StaffDto? staff)> UpdateStaffAsync(int id, StaffCreateRequest request);
        Task<bool> DeleteStaffAsync(int id);
        Task<IEnumerable<StaffDto>> SearchStaffAsync(string keyword);
        Task<PaginatedResponse<StaffDto>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);
    }
}
