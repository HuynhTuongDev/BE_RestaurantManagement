using RestaurantManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IMenuItemImageRepository
    {
        Task<MenuItemImage?> GetByIdAsync(int id);
        Task<IEnumerable<MenuItemImage>> GetByMenuItemIdAsync(int menuItemId);
        Task AddAsync(MenuItemImage menuItemImage);
        Task UpdateAsync(MenuItemImage menuItemImage);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}