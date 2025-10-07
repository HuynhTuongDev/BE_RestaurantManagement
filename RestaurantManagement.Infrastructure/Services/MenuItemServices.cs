using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;

namespace RestaurantManagement.Infrastructure.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        // add dish
        public async Task<MenuItem> AddAsync(MenuItem item)
        {

            await _menuItemRepository.AddAsync(item);
            return item;
        }

        // Delete dish by ID
        public async Task DeleteAsync(int id)
        {
            var dish = await _menuItemRepository.GetByIdAsync(id);
            if (dish == null)
                throw new KeyNotFoundException($"MenuItem {id} không tồn tại.");

            await _menuItemRepository.DeleteAsync(id);
        }

        // Get dish details
        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _menuItemRepository.GetByIdAsync(id);
        }

        // Search by keyword (Name or Description)
        public Task<IEnumerable<MenuItem>> SearchAsync(string keyword)
        {
            return _menuItemRepository.SearchAsync(keyword);
        }

        // Update dish information
        public async Task UpdateAsync(MenuItem item)
        {
            var existing = await _menuItemRepository.GetByIdAsync(item.Id);
            if (existing == null)
                throw new KeyNotFoundException($"MenuItem {item.Id} không tồn tại.");

            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.Price = item.Price;
            existing.Category = item.Category;

            await _menuItemRepository.UpdateAsync(existing);
        }
    }
}
