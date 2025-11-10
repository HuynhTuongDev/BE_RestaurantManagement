using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services.IUserService;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.DTOs.UserDTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Interfaces.Repositories;

namespace RestaurantManagement.Infrastructure.Services.UserServices
{
    public class CustomerService : ICustomerService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IUserRepository userRepository, ILogger<CustomerService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<CustomerResponse> CreateCustomerAsync(CustomerCreateRequest request)
        {
            try
            {
                // Generate email if not provided (for walk-in customers)
                string email = string.IsNullOrWhiteSpace(request.Email)
                    ? $"customer_{DateTime.UtcNow.Ticks}@temp.local"
                    : request.Email;

                // Check if email already exists
                if (await _userRepository.EmailExistsAsync(email))
                {
                    return new CustomerResponse { Success = false, Message = "Email already exists" };
                }

                var user = new User
                {
                    FullName = request.FullName,
                    Email = email,
                    Phone = request.Phone,
                    Address = request.Address,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("customer123"), // Default password
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);

                return new CustomerResponse
                {
                    Success = true,
                    Message = "Customer created successfully",
                    Customer = MapToCustomerDto(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return new CustomerResponse { Success = false, Message = "An error occurred while creating customer" };
            }
        }

        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _userRepository.GetByRoleAsync(UserRole.Customer);
                return customers.Select(MapToCustomerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all customers");
                return new List<CustomerDto>();
            }
        }

        /// <summary>
        /// Get paginated customers
        /// </summary>
        public async Task<PaginatedResponse<CustomerDto>> GetPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Getting paginated Customers - Page: {PageNumber}, Size: {PageSize}",
                    pagination.PageNumber,
                    pagination.PageSize);

                // Get all customers first (filter by role)
                var allCustomers = await _userRepository.GetByRoleAsync(UserRole.Customer);
                
                // Apply pagination manually
                var totalCount = allCustomers.Count();
                var paginatedData = allCustomers
                    .Skip(pagination.SkipCount)
                    .Take(pagination.PageSize)
                    .Select(MapToCustomerDto)
                    .ToList();

                var result = PaginatedResponse<CustomerDto>.Create(
                    paginatedData,
                    pagination.PageNumber,
                    pagination.PageSize,
                    totalCount);

                _logger.LogInformation(
                    "Retrieved {Count} customers out of {Total} - Page {PageNumber}/{TotalPages}",
                    result.Data.Count(),
                    result.TotalRecords,
                    result.PageNumber,
                    result.TotalPages);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated Customers");
                throw;
            }
        }

        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user?.Role == UserRole.Customer)
                {
                    return MapToCustomerDto(user);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customer {Id}", id);
                return null;
            }
        }

        public async Task<CustomerResponse> UpdateCustomerAsync(int id, CustomerCreateRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user?.Role != UserRole.Customer)
                {
                    return new CustomerResponse { Success = false, Message = "Customer not found" };
                }

                // Check email exists for other users
                if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email && 
                    await _userRepository.EmailExistsAsync(request.Email))
                {
                    return new CustomerResponse { Success = false, Message = "Email already exists" };
                }

                user.FullName = request.FullName;
                if (!string.IsNullOrEmpty(request.Email))
                {
                    user.Email = request.Email;
                }
                user.Phone = request.Phone;
                user.Address = request.Address;
                user.UpdatedAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);

                return new CustomerResponse
                {
                    Success = true,
                    Message = "Customer updated successfully",
                    Customer = MapToCustomerDto(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {Id}", id);
                return new CustomerResponse { Success = false, Message = "An error occurred while updating customer" };
            }
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user?.Role == UserRole.Customer)
                {
                    return await _userRepository.SoftDeleteUserAsync(id);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {Id}", id);
                return false;
            }
        }

        public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string keyword)
        {
            try
            {
                var customers = await _userRepository.SearchByKeywordAsync(keyword, UserRole.Customer);
                return customers.Select(MapToCustomerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching customers with keyword {Keyword}", keyword);
                return new List<CustomerDto>();
            }
        }

        /// <summary>
        /// Search customers with pagination
        /// </summary>
        public async Task<PaginatedResponse<CustomerDto>> SearchPaginatedAsync(
            string keyword,
            PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Searching paginated Customers with keyword: {Keyword} - Page: {PageNumber}, Size: {PageSize}",
                    keyword,
                    pagination.PageNumber,
                    pagination.PageSize);

                var customers = await _userRepository.SearchByKeywordAsync(keyword, UserRole.Customer);

                // Calculate pagination
                var totalCount = customers.Count();
                var paginatedData = customers
                    .Skip(pagination.SkipCount)
                    .Take(pagination.PageSize)
                    .Select(MapToCustomerDto)
                    .ToList();

                var result = PaginatedResponse<CustomerDto>.Create(
                    paginatedData,
                    pagination.PageNumber,
                    pagination.PageSize,
                    totalCount);

                _logger.LogInformation(
                    "Found {Count} customers out of {Total} matching keyword: {Keyword}",
                    result.Data.Count(),
                    result.TotalRecords,
                    keyword);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching paginated Customers with keyword: {Keyword}", keyword);
                throw;
            }
        }

        private static CustomerDto MapToCustomerDto(User user)
        {
            return new CustomerDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email.Contains("@temp.local") ? string.Empty : user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role.ToString(),
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted
            };
        }
    }
}
