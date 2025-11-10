using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Application.Services
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(OrderCreateRequest request);
        Task<OrderListResponse> GetAllOrdersAsync();
        Task<PaginatedResponse<OrderDto>> GetPaginatedAsync(PaginationRequest pagination);
        Task<OrderDto?> GetOrderByIdAsync(int id, int? userId = null);
        Task<OrderListResponse> SearchOrdersAsync(string keyword);
        Task<PaginatedResponse<OrderDto>> SearchPaginatedAsync(string keyword, PaginationRequest pagination);
        Task<OrderResponse> UpdateOrderAsync(int id, OrderUpdateRequest request);
        Task<bool> CancelOrderAsync(int id, int? userId, bool isCustomerRequest);
        Task<OrderStatus?> GetOrderStatusAsync(int id, int? userId = null);
    }
}
