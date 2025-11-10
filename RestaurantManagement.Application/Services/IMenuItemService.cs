using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItem>> GetAllAsync();
        Task<PaginatedResponse<MenuItem>> GetPaginatedAsync(PaginationRequest pagination);
        Task<IEnumerable<MenuItem>> SearchAsync(string keyword);
        Task<PaginatedResponse<MenuItem>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);
        Task<MenuItem?> GetByIdAsync(int id);
        Task<MenuItem> AddAsync(MenuItem item);
        Task UpdateAsync(MenuItem item);
        Task DeleteAsync(int id);
    }
}
