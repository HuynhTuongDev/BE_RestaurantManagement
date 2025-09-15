using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IStaffRepository
    {
        Task<User> AddAsync(User staff, StaffProfile profile);
    }
}
