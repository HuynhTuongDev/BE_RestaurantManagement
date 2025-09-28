using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IRestaurantTableService
    {
        Task<RestaurantTable?> GetByIdAsync(int id);
        Task<IEnumerable<RestaurantTable>> GetAllAsync();
    }
}
