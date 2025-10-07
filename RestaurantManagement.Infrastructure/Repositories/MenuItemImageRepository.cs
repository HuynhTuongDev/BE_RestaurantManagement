using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class MenuItemImageRepository : IMenuItemImageRepository
    {
        private readonly RestaurantDbContext _context;

        public MenuItemImageRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<MenuItemImage?> GetByIdAsync(int id)
        {
            return await _context.MenuItemImages
                .FirstOrDefaultAsync(img => img.Id == id);
        }

        public async Task<IEnumerable<MenuItemImage>> GetByMenuItemIdAsync(int menuItemId)
        {
            return await _context.MenuItemImages
                .Where(img => img.MenuItemId == menuItemId)
                .ToListAsync();
        }

        public async Task AddAsync(MenuItemImage menuItemImage)
        {
            _context.MenuItemImages.Add(menuItemImage);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(MenuItemImage menuItemImage)
        {
            _context.MenuItemImages.Update(menuItemImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var image = await _context.MenuItemImages.FindAsync(id);
            if (image != null)
            {
                _context.MenuItemImages.Remove(image);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.MenuItemImages.AnyAsync(img => img.Id == id);
        }
    }
}