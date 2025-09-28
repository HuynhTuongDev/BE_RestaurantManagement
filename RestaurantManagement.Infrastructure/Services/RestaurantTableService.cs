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
    public class RestaurantTableService : IRestaurantTableService
    {
        private readonly IRestaurantTableRepository _restaurantTableRepository;
        public RestaurantTableService(IRestaurantTableRepository restaurantTableRepository)
        {
            _restaurantTableRepository = restaurantTableRepository;
        }
        public async Task<RestaurantTable?> GetByIdAsync(int id)
        {
            return await _restaurantTableRepository.GetByIdAsync(id);
        }
        public async Task<IEnumerable<RestaurantTable>> GetAllAsync()
        {
            return await _restaurantTableRepository.GetAllAsync();
        }
    }
}
