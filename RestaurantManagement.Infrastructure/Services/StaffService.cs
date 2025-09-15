using Microsoft.AspNetCore.Identity;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher;

        public StaffService(IStaffRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<StaffResponse> CreateStaffAsync(StaffCreateRequest request)
        {
            var staff = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = _passwordHasher.HashPassword(null!, request.Password),
                Phone = request.Phone,
                Address = request.Address,
                Role = UserRole.Staff,
                Status = UserStatus.Active
            };

            var profile = new StaffProfile
            {
                Position = request.Position,
                HireDate = request.HireDate
            };

            var created = await _repository.AddAsync(staff, profile);

            return new StaffResponse
            {
                Id = created.Id,
                FullName = created.FullName,
                Email = created.Email,
                Position = profile.Position,
                HireDate = profile.HireDate,
                Status = created.Status.ToString()
            };
        }
    }
}
