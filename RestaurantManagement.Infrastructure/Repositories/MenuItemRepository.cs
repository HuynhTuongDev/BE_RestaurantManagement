using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                .Include(m => m.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Include(m => m.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<MenuItem>> GetByCategoryAsync(string category)
        {
            return await _context.MenuItems
                .Include(m => m.Images)
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
    }
}