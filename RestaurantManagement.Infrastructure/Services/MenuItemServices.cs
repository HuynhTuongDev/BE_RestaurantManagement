using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Interfaces.Repositories;

namespace RestaurantManagement.Infrastructure.Services
{
    /// <summary>
    /// Menu Item service implementation with logging
    /// </summary>
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly ILogger<MenuItemService> _logger;

        public MenuItemService(IMenuItemRepository menuItemRepository, ILogger<MenuItemService> logger)
        {
            _menuItemRepository = menuItemRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all menu items
        /// </summary>
        public async Task<IEnumerable<MenuItemDto>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Getting all MenuItems");
                var items = await _menuItemRepository.GetAllAsync();
                var result = items.Select(MapToDto);
                _logger.LogInformation("Successfully retrieved {Count} MenuItems", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all MenuItems");
                throw;
            }
        }

        /// <summary>
        /// Get paginated menu items
        /// </summary>
        public async Task<PaginatedResponse<MenuItemDto>> GetPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Getting paginated MenuItems - Page: {PageNumber}, Size: {PageSize}",
                    pagination.PageNumber,
                    pagination.PageSize);

                // Cast to base repository
                var baseRepo = _menuItemRepository as IBaseRepository<MenuItem>;
                if (baseRepo == null)
                {
                    _logger.LogError("Repository does not implement IBaseRepository");
                    throw new InvalidOperationException("Repository does not support pagination");
                }

                var paginatedItems = await baseRepo.GetPaginatedAsync(pagination);

                // Map to DTOs
                var mappedData = paginatedItems.Data.Select(MapToDto);
                var result = PaginatedResponse<MenuItemDto>.Create(
                    mappedData,
                    paginatedItems.PageNumber,
                    paginatedItems.PageSize,
                    paginatedItems.TotalRecords);

                _logger.LogInformation(
                    "Retrieved {Count} menu items out of {Total} - Page {PageNumber}/{TotalPages}",
                    result.Data.Count(),
                    result.TotalRecords,
                    result.PageNumber,
                    result.TotalPages);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated MenuItems");
                throw;
            }
        }

        /// <summary>
        /// Add menu item
        /// </summary>
        public async Task<MenuItemDto> AddAsync(MenuItemCreateDto itemDto)
        {
            try
            {
                _logger.LogInformation("Creating MenuItem: {Name}", itemDto.Name);

                ValidateMenuItemDto(itemDto);

                var item = new MenuItem
                {
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Price = itemDto.Price,
                    Category = itemDto.Category,
                    Status = MenuItemStatus.Available
                };

                await _menuItemRepository.AddAsync(item);

                _logger.LogInformation("Successfully created MenuItem: {Name}", item.Name);

                return MapToDto(item);
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
        public async Task<MenuItemDto?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting MenuItem {MenuItemId}", id);

                var item = await _menuItemRepository.GetByIdAsync(id);
                
                if (item == null)
                {
                    _logger.LogWarning("MenuItem {MenuItemId} not found", id);
                    return null;
                }

                _logger.LogInformation("Successfully retrieved MenuItem {MenuItemId}", id);
                return MapToDto(item);
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
        public async Task<IEnumerable<MenuItemDto>> SearchAsync(string keyword)
        {
            try
            {
                _logger.LogInformation("Searching MenuItems with keyword: {Keyword}", keyword);
                
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _logger.LogWarning("Search keyword is empty");
                    return new List<MenuItemDto>();
                }

                var items = await _menuItemRepository.SearchAsync(keyword);
                var result = items.Select(MapToDto);
                _logger.LogInformation("Search found {Count} MenuItems", result.Count());
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching MenuItems with keyword: {Keyword}", keyword);
                throw;
            }
        }

        /// <summary>
        /// Search menu items with pagination
        /// </summary>
        public async Task<PaginatedResponse<MenuItemDto>> SearchPaginatedAsync(
            string keyword,
            PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Searching paginated MenuItems with keyword: {Keyword} - Page: {PageNumber}, Size: {PageSize}",
                    keyword,
                    pagination.PageNumber,
                    pagination.PageSize);

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _logger.LogWarning("Search keyword is empty");
                    return PaginatedResponse<MenuItemDto>.Create(
                        new List<MenuItemDto>(),
                        pagination.PageNumber,
                        pagination.PageSize,
                        0);
                }

                var allItems = await _menuItemRepository.SearchAsync(keyword);

                // Calculate pagination and map to DTOs
                var totalCount = allItems.Count();
                var paginatedData = allItems
                    .Skip(pagination.SkipCount)
                    .Take(pagination.PageSize)
                    .Select(MapToDto)
                    .ToList();

                var result = PaginatedResponse<MenuItemDto>.Create(
                    paginatedData,
                    pagination.PageNumber,
                    pagination.PageSize,
                    totalCount);

                _logger.LogInformation(
                    "Found {Count} menu items out of {Total} matching keyword: {Keyword}",
                    result.Data.Count(),
                    result.TotalRecords,
                    keyword);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching paginated MenuItems with keyword: {Keyword}", keyword);
                throw;
            }
        }

        /// <summary>
        /// Update menu item
        /// </summary>
        public async Task<MenuItemDto> UpdateAsync(int id, MenuItemCreateDto itemDto)
        {
            try
            {
                _logger.LogInformation("Updating MenuItem {MenuItemId}", id);

                var existing = await _menuItemRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("MenuItem {MenuItemId} not found", id);
                    throw new KeyNotFoundException($"MenuItem {id} not found");
                }

                ValidateMenuItemDto(itemDto);

                existing.Name = itemDto.Name;
                existing.Description = itemDto.Description;
                existing.Price = itemDto.Price;
                existing.Category = itemDto.Category;

                await _menuItemRepository.UpdateAsync(existing);

                _logger.LogInformation("Successfully updated MenuItem {MenuItemId}", id);
                
                return MapToDto(existing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating MenuItem {MenuItemId}", id);
                throw;
            }
        }

        /// <summary>
        /// Map MenuItem entity to DTO
        /// </summary>
        private static MenuItemDto MapToDto(MenuItem item)
        {
            return new MenuItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Category = item.Category,
                Status = item.Status.ToString(),
                Images = item.Images?.Select(img => new MenuItemImageDto
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    MenuItemId = img.MenuItemId
                }).ToList() ?? new List<MenuItemImageDto>()
            };
        }

        /// <summary>
        /// Validate menu item DTO
        /// </summary>
        private void ValidateMenuItemDto(MenuItemCreateDto itemDto)
        {
            _logger.LogDebug("Validating MenuItemDto");

            if (string.IsNullOrWhiteSpace(itemDto.Name))
                throw new ArgumentException("MenuItem name is required");

            if (itemDto.Price <= 0)
                throw new ArgumentException("MenuItem price must be greater than 0");

            if (string.IsNullOrWhiteSpace(itemDto.Category))
                throw new ArgumentException("MenuItem category is required");
        }
    }
}
