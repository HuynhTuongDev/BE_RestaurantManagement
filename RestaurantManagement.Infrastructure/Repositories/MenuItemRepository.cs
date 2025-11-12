using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Menu Item repository implementation
    /// </summary>
    public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(RestaurantDbContext context, ILogger<MenuItemRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Get menu items by category
        /// </summary>
        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(string category)
        {
            try
            {
                Logger.LogInformation("Getting MenuItems by category: {Category}", category);
                return await DbSet
                    .Where(m => m.Category == category)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting MenuItems by category: {Category}", category);
                throw;
            }
        }

        /// <summary>
        /// Add menu item (explicit implementation)
        /// </summary>
        async Task IMenuItemRepository.AddAsync(MenuItem menuItem)
        {
            await CreateAsync(menuItem);
        }

        /// <summary>
        /// Update menu item (explicit implementation)
        /// </summary>
        async Task IMenuItemRepository.UpdateAsync(MenuItem menuItem)
        {
            await UpdateAsync(menuItem);
        }

        /// <summary>
        /// Delete menu item (explicit implementation)
        /// </summary>
        async Task IMenuItemRepository.DeleteAsync(int id)
        {
            await DeleteAsync(id);
        }

        /// <summary>
        /// Override search to include specific search logic for MenuItems
        /// </summary>
        public override async Task<IEnumerable<MenuItem>> SearchAsync(string keyword)
        {
            try
            {
                Logger.LogInformation("Searching MenuItems with keyword: {Keyword}", keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Logger.LogWarning("Search keyword is empty");
                    return new List<MenuItem>();
                }

                var searchTerm = keyword.Trim().ToLower();
                
                return await DbSet
                    .Where(m =>
                        m.Name.ToLower().Contains(searchTerm) ||
                        (m.Description != null && m.Description.ToLower().Contains(searchTerm)) ||
                        (m.Category != null && m.Category.ToLower().Contains(searchTerm)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching MenuItems with keyword: {Keyword}", keyword);
                throw;
            }
        }
    }
}
