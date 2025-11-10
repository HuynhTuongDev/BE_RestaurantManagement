using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Infrastructure.Services
{
    /// <summary>
    /// Menu Item service implementation with logging and validation
    /// </summary>
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ILogger<MenuItemService> _logger;

        public MenuItemService(
            IMenuItemRepository menuItemRepository,
            ILogger<MenuItemService> logger)
        {
            _menuItemRepository = menuItemRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all menu items
        /// </summary>
        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all MenuItems");
                var items = await _menuItemRepository.GetAllAsync();
                _logger.LogInformation("Successfully retrieved {Count} MenuItems", items.Count());
                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all MenuItems");
                throw;
            }
        }

        /// <summary>
        /// Add menu item
        /// </summary>
        public async Task<MenuItem> AddAsync(MenuItem item)
        {
            try
            {
                _logger.LogInformation("Creating MenuItem: {Name}", item.Name);

                ValidateMenuItem(item);

                await _menuItemRepository.AddAsync(item);

                _logger.LogInformation("Successfully created MenuItem: {Name}", item.Name);

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating MenuItem");
                throw;
            }
        }

        /// <summary>
        /// Delete menu item by id
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting MenuItem {MenuItemId}", id);

                var item = await _menuItemRepository.GetByIdAsync(id);
                if (item == null)
                {
                    _logger.LogWarning("MenuItem {MenuItemId} not found", id);
                    throw new KeyNotFoundException($"MenuItem {id} not found");
                }

                await _menuItemRepository.DeleteAsync(id);

                _logger.LogInformation("Successfully deleted MenuItem {MenuItemId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting MenuItem {MenuItemId}", id);
                throw;
            }
        }

        /// <summary>
        /// Get menu item by id
        /// </summary>
        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting MenuItem {MenuItemId}", id);

                var item = await _menuItemRepository.GetByIdAsync(id);
                
                if (item == null)
                    _logger.LogWarning("MenuItem {MenuItemId} not found", id);
                else
                    _logger.LogInformation("Successfully retrieved MenuItem {MenuItemId}", id);

                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting MenuItem {MenuItemId}", id);
                throw;
            }
        }

        /// <summary>
        /// Search menu items by keyword
        /// </summary>
        public async Task<IEnumerable<MenuItem>> SearchAsync(string keyword)
        {
            try
            {
                _logger.LogInformation("Searching MenuItems with keyword: {Keyword}", keyword);

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _logger.LogWarning("Search keyword is empty");
                    return new List<MenuItem>();
                }

                var items = await _menuItemRepository.SearchAsync(keyword);

                _logger.LogInformation("Search found {Count} MenuItems matching keyword: {Keyword}", 
                    items.Count(), keyword);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching MenuItems with keyword: {Keyword}", keyword);
                throw;
            }
        }

        /// <summary>
        /// Update menu item
        /// </summary>
        public async Task UpdateAsync(MenuItem item)
        {
            try
            {
                _logger.LogInformation("Updating MenuItem {MenuItemId}", item.Id);

                var existing = await _menuItemRepository.GetByIdAsync(item.Id);
                if (existing == null)
                {
                    _logger.LogWarning("MenuItem {MenuItemId} not found", item.Id);
                    throw new KeyNotFoundException($"MenuItem {item.Id} not found");
                }

                ValidateMenuItem(item);

                existing.Name = item.Name;
                existing.Description = item.Description;
                existing.Price = item.Price;
                existing.Category = item.Category;

                await _menuItemRepository.UpdateAsync(existing);

                _logger.LogInformation("Successfully updated MenuItem {MenuItemId}", item.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating MenuItem {MenuItemId}", item.Id);
                throw;
            }
        }

        /// <summary>
        /// Validate menu item
        /// </summary>
        private void ValidateMenuItem(MenuItem item)
        {
            _logger.LogDebug("Validating MenuItem");

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("MenuItem name is required");

            if (item.Price <= 0)
                throw new ArgumentException("MenuItem price must be greater than 0");

            if (string.IsNullOrWhiteSpace(item.Category))
                throw new ArgumentException("MenuItem category is required");
        }
    }
}
