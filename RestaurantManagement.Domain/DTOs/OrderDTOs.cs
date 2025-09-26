using RestaurantManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.DTOs
{
    public class OrderItemDto
    {
        public int MenuItemId { get; set; }
        public string? MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public DateTime OrderTime { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderCreateRequest
    {
        [Required]
        public int TableId { get; set; }

        [Required]
        [MinLength(1)]
        public List<OrderItemRequest> Items { get; set; } = new();

        [JsonIgnore]
        public int UserId { get; set; }
    }

    public class OrderItemRequest
    {
        [Required]
        public int MenuItemId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class OrderUpdateRequest
    {
        [Required]
        [MinLength(1)]
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public OrderDto? Order { get; set; }
    }

    public class OrderListResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public IEnumerable<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}
