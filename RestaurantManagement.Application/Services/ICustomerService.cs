using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Services
{
    public interface ICustomerService
    {
        Task<User> CreateCustomerAsync(CustomerCreateRequest request);
        Task<IEnumerable<User>> SearchCustomersAsync(string keyword);
        Task<IEnumerable<User>> GetAllCustomersAsync();


    }
}
