using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IRestaurantTableService
    {
        Task<RestaurantTable?> GetByIdAsync(int id);
        Task<IEnumerable<RestaurantTable>> GetAllAsync();

        Task<IEnumerable<RestaurantTable>> SearchAsync(string keyword);
        Task<RestaurantTable> AddAsync(RestaurantTableCreateDto dto);
        Task UpdateAsync(int id, RestaurantTableCreateDto dto);
        Task DeleteAsync(int id);
        Task<bool> ReserveAsync(int id);    
        Task<bool> CancelReservationAsync(int id);
    }
}
