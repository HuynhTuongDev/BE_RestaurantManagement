using RestaurantManagement.Domain.DTOs.UserDTOs;

namespace RestaurantManagement.Application.Services.IUserService
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateCustomerAsync(CustomerCreateRequest request);
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto?> GetCustomerByIdAsync(int id);
        Task<CustomerResponse> UpdateCustomerAsync(int id, CustomerCreateRequest request);
        Task<bool> DeleteCustomerAsync(int id);
        Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string keyword);
    }   
}
