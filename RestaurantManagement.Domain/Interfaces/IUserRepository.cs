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
    }
}
