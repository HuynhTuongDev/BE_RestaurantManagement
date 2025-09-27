using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class RestaurantTableRepository : IRestaurantTableRepository
    {
        private readonly RestaurantDbContext _context;

        public RestaurantTableRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<RestaurantTable?> GetByIdAsync(int id)
        {
            return await _context.RestaurantTables.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<RestaurantTable>> GetAllAsync()
        {
            return await _context.RestaurantTables.ToListAsync();
        }
        public async Task<IEnumerable<RestaurantTable>> SearchAsync(string keyword)
        {
            return await _context.RestaurantTables
                .Where(t => t.TableNumber.ToString().Contains(keyword) || t.Location.Contains(keyword))
                .ToListAsync();
        }
        

        public async Task AddAsync(RestaurantTable restaurantTable)
        {
            await _context.RestaurantTables.AddAsync(restaurantTable);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(RestaurantTable restaurantTable)
        {
            _context.RestaurantTables.Update(restaurantTable);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var table = await GetByIdAsync(id);
            if (table != null)
            {
                _context.RestaurantTables.Remove(table);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id) =>
            await _context.RestaurantTables.AnyAsync(t => t.Id == id);
    
    }
}
