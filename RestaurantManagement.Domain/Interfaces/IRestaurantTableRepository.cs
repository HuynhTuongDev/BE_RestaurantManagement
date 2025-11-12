using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IRestaurantTableRepository
    {
        Task<RestaurantTable?> GetByIdAsync(int id);
        Task<IEnumerable<RestaurantTable>> GetAllAsync();
        
        // search by TableNumber or Location
        Task<IEnumerable<RestaurantTable>> SearchAsync(int TableNumber); 
        Task AddAsync(RestaurantTable restaurantTable);
        Task UpdateAsync(RestaurantTable restaurantTable);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
