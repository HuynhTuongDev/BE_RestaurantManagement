using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Order repository implementation
    /// </summary>
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(RestaurantDbContext context, ILogger<OrderRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Add order (explicit implementation)
        /// </summary>
        async Task IOrderRepository.AddAsync(Order order)
        {
            await CreateAsync(order);
        }

        /// <summary>
        /// Update order (explicit implementation)
        /// </summary>
        async Task IOrderRepository.UpdateAsync(Order order)
        {
            await UpdateAsync(order);
        }

        /// <summary>
        /// Get order by id with all details (including order items)
        /// </summary>
        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                Logger.LogInformation("Getting Order {OrderId} with details", id);
                
                return await DbSet
                    .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.MenuItem)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Order {OrderId} with details", id);
                throw;
            }
        }

        /// <summary>
        /// Get all orders with details
        /// </summary>
        public override async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                Logger.LogInformation("Getting all Orders with details");
                
                return await DbSet
                    .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.MenuItem)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting all Orders");
                throw;
            }
        }

        /// <summary>
        /// Search orders by keyword (order id, table id, or user name)
        /// </summary>
        public async Task<IEnumerable<Order>> SearchByKeywordAsync(string keyword)
        {
            try
            {
                Logger.LogInformation("Searching Orders with keyword: {Keyword}", keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Logger.LogWarning("Search keyword is empty");
                    return new List<Order>();
                }

                var searchTerm = keyword.Trim().ToLower();

                return await DbSet
                    .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.MenuItem)
                    .Include(o => o.User)
                    .Where(o =>
                        o.Id.ToString().Contains(searchTerm) ||
                        o.TableId.ToString().Contains(searchTerm) ||
                        o.User.FullName.ToLower().Contains(searchTerm))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Orders with keyword: {Keyword}", keyword);
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
