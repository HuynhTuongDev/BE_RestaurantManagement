using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// MenuItem Image repository implementation
    /// </summary>
    public class MenuItemImageRepository : BaseRepository<MenuItemImage>, IMenuItemImageRepository
    {
        public MenuItemImageRepository(RestaurantDbContext context, ILogger<MenuItemImageRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Get images by menu item id
        /// </summary>
        public async Task<IEnumerable<MenuItemImage>> GetByMenuItemIdAsync(int menuItemId)
        {
            try
            {
                Logger.LogInformation("Getting MenuItemImages for MenuItem {MenuItemId}", menuItemId);
                
                return await DbSet
                    .Where(img => img.MenuItemId == menuItemId)
                    .ToListAsync();
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "Error getting MenuItemImages for MenuItem {MenuItemId}", menuItemId);
                throw;
            }
        }

        /// <summary>
        /// Add image (explicit implementation)
        /// </summary>
        async Task IMenuItemImageRepository.AddAsync(MenuItemImage menuItemImage)
        {
            await CreateAsync(menuItemImage);
        }

        /// <summary>
        /// Update image (explicit implementation)
        /// </summary>
        async Task IMenuItemImageRepository.UpdateAsync(MenuItemImage menuItemImage)
        {
            await UpdateAsync(menuItemImage);
        }

        /// <summary>
        /// Delete image (explicit implementation)
        /// </summary>
        async Task IMenuItemImageRepository.DeleteAsync(int id)
        {
            await DeleteAsync(id);
        }

        /// <summary>
        /// Override search for images
        /// </summary>
        public override async Task<IEnumerable<MenuItemImage>> SearchAsync(string keyword)
        {
            try
            {
                Logger.LogInformation("Searching MenuItemImages with keyword: {Keyword}", keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Logger.LogWarning("Search keyword is empty");
                    return new List<MenuItemImage>();
                }

                if (!int.TryParse(keyword, out var menuItemId))
                {
                    Logger.LogWarning("Invalid menu item id: {Keyword}", keyword);
                    return new List<MenuItemImage>();
                }

                return await DbSet
                    .Where(img => img.MenuItemId == menuItemId)
                    .ToListAsync();
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "Error searching MenuItemImages with keyword: {Keyword}", keyword);
                throw;
            }
        }
    }
}