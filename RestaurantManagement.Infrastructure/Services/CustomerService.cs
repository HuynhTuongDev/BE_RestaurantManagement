using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Services
{

    public class CustomerService : ICustomerService
    {
        private readonly CustomerRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher;

        public CustomerService(CustomerRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<User> CreateCustomerAsync(CustomerCreateRequest request)
        {
            // Nếu không nhập email thì dùng mặc định
            string email = string.IsNullOrWhiteSpace(request.Email)
                ? "customer@gmail.com"
                : request.Email;

            var customer = new User
            {
                FullName = request.FullName,
                Email = email,
                Phone = request.Phone,
                Address = request.Address,
                PasswordHash = _passwordHasher.HashPassword(null!, "12345678"),
                Role = UserRole.Customer,
                Status = UserStatus.Active
            };

            return await _repository.AddAsync(customer);
        }

        public async Task<IEnumerable<User>> SearchCustomersAsync(string keyword)
        {
            return await _repository.SearchAsync(keyword);
        }
        public async Task<IEnumerable<User>> GetAllCustomersAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}
