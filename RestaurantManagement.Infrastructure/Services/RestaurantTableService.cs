using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
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

        public async Task<IEnumerable<RestaurantTable>> SearchAsync(string keyword)
        {
            return await _restaurantTableRepository.SearchAsync(keyword);
        } 

        public async Task<RestaurantTable> AddAsync(RestaurantTableCreateDto dto)
        {
            var restaurantTable = new RestaurantTable
            {
                TableNumber = dto.TableNumber,
                Seats = dto.Seats,
                Status = dto.Status,
                Location = dto.Location
            };
            await _restaurantTableRepository.AddAsync(restaurantTable);
            return restaurantTable;
        }

        public async Task UpdateAsync(int id, RestaurantTableCreateDto dto)
        {
            var table = await _restaurantTableRepository.GetByIdAsync(id);
            if (table == null) throw new KeyNotFoundException("Table not found.");

            table.TableNumber = dto.TableNumber;
            table.Seats = dto.Seats;
            table.Status = dto.Status;
            table.Location = dto.Location;
            await _restaurantTableRepository.UpdateAsync(table);
        }

        public async Task DeleteAsync(int id) => await _restaurantTableRepository.DeleteAsync(id);

        public async Task<bool> ReserveAsync(int id)
        {
            var table = await _restaurantTableRepository.GetByIdAsync(id);
            if (table == null) return false;
            table.Status = TableStatus.Reserved;
            await _restaurantTableRepository.UpdateAsync(table);
            return true;
        }

        public async Task<bool> CancelReservationAsync(int id)
        {
            var table = await _restaurantTableRepository.GetByIdAsync(id);
            if (table == null) return false;
            table.Status = TableStatus.Available;
            await _restaurantTableRepository.UpdateAsync(table);
            return true;
        }
    }
}
