using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class CustomerRepository
    {
        private readonly RestaurantDbContext _context;

        public CustomerRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User customer)
        {
            _context.Users.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<IEnumerable<User>> SearchAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<User>();

            return await _context.Users
                .Where(c => c.Role == UserRole.Customer &&
                           (c.FullName.Contains(keyword) || c.Email.Contains(keyword)))
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Customer && !u.IsDeleted)
                .ToListAsync();
        }

    }
}
