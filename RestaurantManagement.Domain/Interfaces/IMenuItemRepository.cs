using RestaurantManagement.Domain.Entities;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IMenuItemRepository
    {
        Task<MenuItem?> GetByIdAsync(int id);
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<IEnumerable<MenuItem>> GetByCategoryAsync(string category);
        Task AddAsync(MenuItem menuItem);
        Task UpdateAsync(MenuItem menuItem);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<MenuItem>> SearchAsync(string keyword);
        
    }
}