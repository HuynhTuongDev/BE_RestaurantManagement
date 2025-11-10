using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// User repository implementation
    /// </summary>
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(RestaurantDbContext context, ILogger<UserRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Add user (explicit implementation)
        /// </summary>
        async Task IUserRepository.AddAsync(User user)
        {
            await CreateAsync(user);
        }

        /// <summary>
        /// Update user (explicit implementation)
        /// </summary>
        async Task IUserRepository.UpdateAsync(User user)
        {
            await UpdateAsync(user);
        }

        /// <summary>
        /// Get user by email
        /// </summary>
        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                Logger.LogInformation("Getting User with email: {Email}", email);
                
                if (string.IsNullOrWhiteSpace(email))
                {
                    Logger.LogWarning("Email is empty");
                    return null;
                }

                return await DbSet.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting User with email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Check if email exists
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                Logger.LogInformation("Checking if email exists: {Email}", email);
                return await DbSet.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error checking email existence: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Soft delete user
        /// </summary>
        public async Task<bool> SoftDeleteUserAsync(int userId)
        {
            try
            {
                Logger.LogInformation("Soft deleting User {UserId}", userId);
                
                var user = await DbSet.FindAsync(userId);
                if (user == null)
                {
                    Logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                user.IsDeleted = true;
                user.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(user);
                
                Logger.LogInformation("Successfully soft deleted User {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error soft deleting User {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Lock user (suspend)
        /// </summary>
        public async Task<bool> LockUserAsync(int userId)
        {
            try
            {
                Logger.LogInformation("Locking User {UserId}", userId);
                
                var user = await DbSet.FindAsync(userId);
                if (user == null)
                {
                    Logger.LogWarning("User {UserId} not found", userId);
                    return false;
                }

                user.Status = UserStatus.Suspended;
                user.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(user);
                
                Logger.LogInformation("Successfully locked User {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error locking User {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Get users by role
        /// </summary>
        public async Task<IEnumerable<User>> GetByRoleAsync(UserRole role)
        {
            try
            {
                Logger.LogInformation("Getting Users with role: {Role}", role);
                
                return await DbSet
                    .Include(u => u.StaffProfile)
                    .Where(u => u.Role == role && !u.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Users with role: {Role}", role);
                throw;
            }
        }

        /// <summary>
        /// Search users by keyword and optional role
        /// </summary>
        public override async Task<IEnumerable<User>> SearchAsync(string keyword)
        {
            return await SearchByKeywordAsync(keyword);
        }

        /// <summary>
        /// Search users with role filter
        /// </summary>
        public async Task<IEnumerable<User>> SearchByKeywordAsync(string keyword, UserRole? role = null)
        {
            try
            {
                Logger.LogInformation("Searching Users with keyword: {Keyword}, role: {Role}", keyword, role);
                
                var query = DbSet
                    .Include(u => u.StaffProfile)
                    .Where(u => !u.IsDeleted);

                if (role.HasValue)
                {
                    query = query.Where(u => u.Role == role.Value);
                }

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    var searchTerm = keyword.ToLower();
                    query = query.Where(u =>
                        u.FullName.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        (u.Phone != null && u.Phone.Contains(searchTerm)) ||
                        (u.StaffProfile != null && u.StaffProfile.Position.ToLower().Contains(searchTerm))
                    );
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Users with keyword: {Keyword}", keyword);
                throw;
            }
        }

        /// <summary>
        /// Get user by id with staff profile
        /// </summary>
        public async Task<User?> GetByIdWithProfileAsync(int id)
        {
            try
            {
                Logger.LogInformation("Getting User {UserId} with profile", id);
                
                return await DbSet
                    .Include(u => u.StaffProfile)
                    .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting User {UserId} with profile", id);
                throw;
            }
        }

        /// <summary>
        /// Save changes
        /// </summary>
        public async Task SaveChangesAsync()
        {
            try
            {
                Logger.LogInformation("Saving changes to database");
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error saving changes to database");
                throw;
            }
        }
    }
}
