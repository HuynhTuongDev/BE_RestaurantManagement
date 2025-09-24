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
    }
}
