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
    public class StaffRepository : IStaffRepository
    {
        private readonly RestaurantDbContext _context;

        public StaffRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User staff, StaffProfile profile)
        {
            _context.Users.Add(staff);
            await _context.SaveChangesAsync();


            profile.UserId = staff.Id;
            _context.StaffProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return staff;
        }
    }
}
