using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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

    }
}
