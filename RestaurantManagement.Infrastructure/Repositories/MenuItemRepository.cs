using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly RestaurantDbContext _context;

        public MenuItemRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(string category)
        {
            return await _context.MenuItems
                .Where(m => m.Category == category)
                .ToListAsync();
        }

        public async Task AddAsync(MenuItem menuItem)
        {
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MenuItems.AnyAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> SearchAsync(string keyword)
        {
            return await _context.MenuItems
                .Where(m =>
                    m.Name.Contains(keyword) ||
                    (m.Description != null && m.Description.Contains(keyword)))
                .ToListAsync();
        }
    }
}
