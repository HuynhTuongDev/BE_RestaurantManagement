using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Restaurant Table repository implementation
    /// </summary>
    public class RestaurantTableRepository : BaseRepository<RestaurantTable>, IRestaurantTableRepository
    {
        public RestaurantTableRepository(RestaurantDbContext context, ILogger<RestaurantTableRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Add table (explicit implementation)
        /// </summary>
        async Task IRestaurantTableRepository.AddAsync(RestaurantTable restaurantTable)
        {
            await CreateAsync(restaurantTable);
        }

        /// <summary>
        /// Update table (explicit implementation)
        /// </summary>
        async Task IRestaurantTableRepository.UpdateAsync(RestaurantTable restaurantTable)
        {
            await UpdateAsync(restaurantTable);
        }

        /// <summary>
        /// Delete table (explicit implementation)
        /// </summary>
        async Task IRestaurantTableRepository.DeleteAsync(int id)
        {
            await DeleteAsync(id);
        }

        /// <summary>
        /// Search tables by table number
        /// </summary>
        public async Task<IEnumerable<RestaurantTable>> SearchAsync(int tableNumber)
        {
            try
            {
                Logger.LogInformation("Searching RestaurantTables with table number: {TableNumber}", tableNumber);
                
                return await DbSet
                    .Where(t => t.TableNumber == tableNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching RestaurantTables with table number: {TableNumber}", tableNumber);
                throw;
            }
        }

        /// <summary>
        /// Get tables by status
        /// </summary>
        public async Task<IEnumerable<RestaurantTable>> GetByStatusAsync(TableStatus status)
        {
            try
            {
                Logger.LogInformation("Getting RestaurantTables with status: {Status}", status);
                
                return await DbSet
                    .Where(t => t.Status == status)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting RestaurantTables with status: {Status}", status);
                throw;
            }
        }

        /// <summary>
        /// Get available tables
        /// </summary>
        public async Task<IEnumerable<RestaurantTable>> GetAvailableTablesAsync()
        {
            try
            {
                Logger.LogInformation("Getting available RestaurantTables");
                
                return await DbSet
                    .Where(t => t.Status == TableStatus.Available)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting available RestaurantTables");
                throw;
            }
        }

        /// <summary>
        /// Get tables by number of seats
        /// </summary>
        public async Task<IEnumerable<RestaurantTable>> GetBySeatsAsync(int seats)
        {
            try
            {
                Logger.LogInformation("Getting RestaurantTables with {Seats} seats", seats);
                
                return await DbSet
                    .Where(t => t.Seats >= seats)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting RestaurantTables with {Seats} seats", seats);
                throw;
            }
        }
    }
}
