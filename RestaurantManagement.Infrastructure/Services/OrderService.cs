using Microsoft.Extensions.Logging;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<OrderResponse> CreateOrderAsync(OrderCreateRequest request)
        {
            try
            {
                var order = new Order
                {
                    UserId = request.UserId,
                    TableId = request.TableId,
                    Status = OrderStatus.Pending,
                    OrderTime = DateTime.UtcNow
                };

                foreach (var item in request.Items)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity
                    });
                }

                await _orderRepository.AddAsync(order);
                await _orderRepository.SaveChangesAsync();

                return new OrderResponse
                {
                    Success = true,
                    Message = "Order created successfully",
                    Order = MapToDto(order)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return new OrderResponse { Success = false, Message = "Failed to create order" };
            }
        }

        public async Task<OrderListResponse> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            return new OrderListResponse
            {
                Success = true,
                Orders = orders.Select(MapToDto)
            };
        }

        /// <summary>
        /// Get paginated orders
        /// </summary>
        public async Task<PaginatedResponse<OrderDto>> GetPaginatedAsync(PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Getting paginated Orders - Page: {PageNumber}, Size: {PageSize}",
                    pagination.PageNumber,
                    pagination.PageSize);

                // Cast to base repository
                var baseRepo = _orderRepository as IBaseRepository<Order>;
                if (baseRepo == null)
                {
                    _logger.LogError("Repository does not implement IBaseRepository");
                    throw new InvalidOperationException("Repository does not support pagination");
                }

                var paginatedOrders = await baseRepo.GetPaginatedAsync(pagination);

                // Map to DTOs
                var mappedData = paginatedOrders.Data.Select(MapToDto);

                var result = PaginatedResponse<OrderDto>.Create(
                    mappedData,
                    paginatedOrders.PageNumber,
                    paginatedOrders.PageSize,
                    paginatedOrders.TotalRecords);

                _logger.LogInformation(
                    "Retrieved {Count} orders out of {Total} - Page {PageNumber}/{TotalPages}",
                    result.Data.Count(),
                    result.TotalRecords,
                    result.PageNumber,
                    result.TotalPages);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting paginated Orders");
                throw;
            }
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id, int? userId = null)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(id);
            if (order == null) return null;

            if (userId.HasValue && order.UserId != userId.Value) return null;

            return MapToDto(order);
        }

        public async Task<OrderListResponse> SearchOrdersAsync(string keyword)
        {
            var orders = await _orderRepository.SearchByKeywordAsync(keyword);
            return new OrderListResponse
            {
                Success = true,
                Orders = orders.Select(MapToDto)
            };
        }

        /// <summary>
        /// Search orders with pagination
        /// </summary>
        public async Task<PaginatedResponse<OrderDto>> SearchPaginatedAsync(
            string keyword, 
            PaginationRequest pagination)
        {
            try
            {
                _logger.LogInformation(
                    "Searching paginated Orders with keyword: {Keyword} - Page: {PageNumber}, Size: {PageSize}",
                    keyword,
                    pagination.PageNumber,
                    pagination.PageSize);

                var allOrders = await _orderRepository.SearchByKeywordAsync(keyword);

                // Calculate pagination
                var totalCount = allOrders.Count();
                var paginatedData = allOrders
                    .Skip(pagination.SkipCount)
                    .Take(pagination.PageSize)
                    .Select(MapToDto)
                    .ToList();

                var result = PaginatedResponse<OrderDto>.Create(
                    paginatedData,
                    pagination.PageNumber,
                    pagination.PageSize,
                    totalCount);

                _logger.LogInformation(
                    "Found {Count} orders out of {Total} matching keyword: {Keyword}",
                    result.Data.Count(),
                    result.TotalRecords,
                    keyword);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching paginated Orders with keyword: {Keyword}", keyword);
                throw;
            }
        }

        public async Task<OrderResponse> UpdateOrderAsync(int id, OrderUpdateRequest request)
        {
            try
            {
                var order = await _orderRepository.GetByIdWithDetailsAsync(id);
                if (order == null)
                    return new OrderResponse { Success = false, Message = "Order not found" };

                if (order.Status != OrderStatus.Pending)
                    return new OrderResponse { Success = false, Message = "Only pending orders can be updated" };

                order.OrderDetails.Clear();
                foreach (var item in request.Items)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity
                    });
                }

                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveChangesAsync();

                return new OrderResponse
                {
                    Success = true,
                    Message = "Order updated successfully",
                    Order = MapToDto(order)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return new OrderResponse { Success = false, Message = "Failed to update order" };
            }
        }

        public async Task<bool> CancelOrderAsync(int id, int? userId, bool isCustomerRequest)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return false;

            if (isCustomerRequest && order.UserId != userId)
                return false;

            if (order.Status != OrderStatus.Pending)
                return false;

            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();
            return true;
        }

        public async Task<OrderStatus?> GetOrderStatusAsync(int id, int? userId = null)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null) return null;

            if (userId.HasValue && order.UserId != userId.Value) return null;

            return order.Status;
        }

        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TableId = order.TableId,
                OrderTime = order.OrderTime,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.OrderDetails.Select(d => new OrderItemDto
                {
                    MenuItemId = d.MenuItemId,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    MenuItemName = d.MenuItem?.Name
                }).ToList()
            };
        }
    }
}
