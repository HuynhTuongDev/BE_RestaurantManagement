using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<User> AddAsync(User customer);
        Task<IEnumerable<User>> SearchAsync(string keyword);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
