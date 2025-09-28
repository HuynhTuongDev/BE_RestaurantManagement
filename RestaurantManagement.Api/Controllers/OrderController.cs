using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Create a new order (Customer or Staff)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get UserId from JWT token and assign to request
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            request.UserId = userId.Value;

            var result = await _orderService.CreateOrderAsync(request);
            if (result.Success)
                return CreatedAtAction(nameof(GetOrder), new { id = result.Order!.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get a list of all orders (Admin/Staff)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrdersAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get order details by Id (Customer can only view their own orders)
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var userId = GetUserIdFromToken();
            var order = await _orderService.GetOrderByIdAsync(id, User.IsInRole("Customer") ? userId : null);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        /// <summary>
        /// Search orders by keyword (Id, TableId, Customer name)
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> SearchOrders([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("Keyword is required.");

            var result = await _orderService.SearchOrdersAsync(keyword);
            return Ok(result);
        }

        /// <summary>
        /// Update order details (only if status is Pending)
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.UpdateOrderAsync(id, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Cancel an order (Customer can only cancel their own Pending orders)
        /// </summary>
        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = GetUserIdFromToken();
            bool isCustomer = User.IsInRole("Customer");

            var success = await _orderService.CancelOrderAsync(id, userId, isCustomer);
            return success ? Ok(new { message = "Order cancelled." }) : BadRequest(new { message = "Cannot cancel order." });
        }

        /// <summary>
        /// Get the current status of an order
        /// </summary>
        [HttpGet("{id:int}/status")]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var userId = GetUserIdFromToken();
            var status = await _orderService.GetOrderStatusAsync(id, User.IsInRole("Customer") ? userId : null);

            if (!status.HasValue)
                return NotFound();

            return Ok(new { status = status.Value.ToString() });
        }

        /// <summary>
        /// Helper: Extract UserId from JWT token
        /// </summary>
        private int? GetUserIdFromToken()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (int.TryParse(idClaim, out var id))
                return id;

            return null;
        }
    }
}
