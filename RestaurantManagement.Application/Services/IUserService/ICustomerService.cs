using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Application.Services.IUserService
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateCustomerAsync(CustomerCreateRequest request);
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<PaginatedResponse<CustomerDto>> GetPaginatedAsync(PaginationRequest pagination);
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerResponse> UpdateCustomerAsync(int id, CustomerCreateRequest request);
        Task<bool> DeleteCustomerAsync(int id);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string keyword);
        Task<PaginatedResponse<CustomerDto>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);
    }   
}
