using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly RestaurantDbContext _context;

        public OrderRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await Task.CompletedTask;
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.MenuItem)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> SearchByKeywordAsync(string keyword)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(d => d.MenuItem)
                .Where(o =>
                    o.Id.ToString().Contains(keyword) ||
                    o.TableId.ToString().Contains(keyword) ||
                    o.User.FullName.Contains(keyword))
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
