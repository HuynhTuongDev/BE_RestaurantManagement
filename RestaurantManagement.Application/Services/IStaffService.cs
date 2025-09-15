using RestaurantManagement.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Services
{
    public interface IStaffService
    {
        Task<StaffResponse> CreateStaffAsync(StaffCreateRequest request);
    }
}
