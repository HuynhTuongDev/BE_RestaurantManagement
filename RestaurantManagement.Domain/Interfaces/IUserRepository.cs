using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task SaveChangesAsync();
        Task<bool> SoftDeleteUserAsync(int userId);
        Task<bool> LockUserAsync(int userId);

        // Extended methods for Staff and Customer management
        Task<IEnumerable<User>> GetByRoleAsync(UserRole role);
        Task<IEnumerable<User>> SearchByKeywordAsync(string keyword, UserRole? role = null);
        Task<User?> GetByIdWithProfileAsync(int id);
    }
}
