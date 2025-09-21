using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        // Thêm món mới
        public async Task<MenuItem> AddAsync(MenuItem item)
        {
            
            await _menuItemRepository.AddAsync(item);
            return item;
        }

        // Xoá món theo Id
        public async Task DeleteAsync(int id)
        {
            var dish = await _menuItemRepository.GetByIdAsync(id);
            if (dish == null)
                throw new KeyNotFoundException($"MenuItem {id} không tồn tại.");

            await _menuItemRepository.DeleteAsync(id);
        }

        // Lấy chi tiết món
        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _menuItemRepository.GetByIdAsync(id);
        }

        // Tìm kiếm theo keyword (Tên hoặc Mô tả)
        public Task<IEnumerable<MenuItem>> SearchAsync(string keyword)
        {  
            return _menuItemRepository.SearchAsync(keyword);
        }

        // Cập nhật thông tin món
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
