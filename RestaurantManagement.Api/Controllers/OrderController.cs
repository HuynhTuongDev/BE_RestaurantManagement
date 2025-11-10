using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using System.Security.Claims;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/order")]
    [ApiVersion("1.0")]
    [Authorize]
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
            : base(logger)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Create a new order (Customer or Staff)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid order data");

            // Get UserId from JWT token and assign to request
            var userId = GetUserIdFromToken();
            if (userId == null)
                return UnauthorizedResponse("Invalid or missing user token");

            request.UserId = userId.Value;

            var result = await _orderService.CreateOrderAsync(request);
            if (result.Success)
                return CreatedResponse(nameof(GetOrder), result.Order!.Id, result, "Order created successfully");

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Get a list of all orders (Admin/Staff)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return OkListResponse(result.Orders, "Orders retrieved successfully");
        }

        /// <summary>
        /// Get paginated orders (Admin/Staff)
        /// </summary>
        [HttpGet("paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginatedOrders([FromQuery] PaginationRequest pagination)
        {
            var paginatedOrders = await _orderService.GetPaginatedAsync(pagination);
            return OkPaginatedResponse(paginatedOrders, "Orders retrieved successfully");
        }

        /// <summary>
        /// Get order details by Id (Customer can only view their own orders)
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserIdFromToken();
            var order = await _orderService.GetOrderByIdAsync(id, User.IsInRole("Customer") ? userId : null);

            if (order == null)
                return NotFoundResponse("Order not found");

            return OkResponse(order, "Order retrieved successfully");
        }

        /// <summary>
        /// Search orders by keyword (Id, TableId, Customer name)
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchOrders([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var result = await _orderService.SearchOrdersAsync(keyword);
            return OkListResponse(result.Orders, "Search completed successfully");
        }

        /// <summary>
        /// Search orders with pagination
        /// </summary>
        [HttpGet("search/paginated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPaginatedOrders(
            [FromQuery] string keyword,
            [FromQuery] PaginationRequest pagination)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequestResponse("Keyword is required");

            var paginatedOrders = await _orderService.SearchPaginatedAsync(keyword, pagination);
            return OkPaginatedResponse(paginatedOrders, "Search completed successfully");
        }

        /// <summary>
        /// Update order details (only if status is Pending)
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid order data");

            var result = await _orderService.UpdateOrderAsync(id, request);
            
            if (result.Success)
                return OkResponse(result, "Order updated successfully");
            
            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Cancel an order (Customer can only cancel their own Pending orders)
        /// </summary>
        [HttpPut("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserIdFromToken();
            bool isCustomer = User.IsInRole("Customer");

            var success = await _orderService.CancelOrderAsync(id, userId, isCustomer);
            
            if (success)
                return OkResponse(new { cancelled = true }, "Order cancelled successfully");
            
            return BadRequestResponse("Cannot cancel order");
        }

        /// <summary>
        /// Get the current status of an order
        /// </summary>
        [HttpGet("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var userId = GetUserIdFromToken();
            var status = await _orderService.GetOrderStatusAsync(id, User.IsInRole("Customer") ? userId : null);

            if (!status.HasValue)
                return NotFoundResponse("Order not found");

            return OkResponse(new { status = status.Value.ToString() }, "Order status retrieved successfully");
        }

        /// <summary>
        /// Helper: Extract UserId from JWT token
        /// </summary>
        private int? GetUserIdFromToken()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id" || c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var id))
                return id;

            return null;
        }
    }
}
