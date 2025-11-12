using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IMenuItemService
    {
        Task<IEnumerable<MenuItemDto>> GetAllAsync();
        Task<PaginatedResponse<MenuItemDto>> GetPaginatedAsync(PaginationRequest pagination);
        Task<IEnumerable<MenuItemDto>> SearchAsync(string keyword);
        Task<PaginatedResponse<MenuItemDto>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);
        Task<MenuItemDto?> GetByIdAsync(int id);
        Task<MenuItemDto> AddAsync(MenuItemCreateDto item);
        Task<MenuItemDto> UpdateAsync(int id, MenuItemCreateDto item);
        Task DeleteAsync(int id);
    }
}
