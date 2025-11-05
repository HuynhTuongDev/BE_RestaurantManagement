using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Create a new payment for an order
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.CreatePaymentAsync(dto);
            if (result.Success)
                return CreatedAtAction(nameof(GetPaymentById), new { id = result.Payment!.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get all payments (Admin/Staff only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            return Ok(payment);
        }

        /// <summary>
        /// Get all payments for a specific order
        /// </summary>
        [HttpGet("order/{orderId:int}")]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return Ok(payments);
        }

        /// <summary>
        /// Get payments by status (Admin/Staff only)
        /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPaymentsByStatus(PaymentStatus status)
        {
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return Ok(payments);
        }

        /// <summary>
        /// Update payment status (Admin only)
        /// </summary>
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.UpdatePaymentStatusAsync(id, dto.Status);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Delete payment (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var success = await _paymentService.DeletePaymentAsync(id);
            if (success)
                return Ok(new { message = "Payment deleted successfully" });

            return NotFound(new { message = "Payment not found" });
        }

        /// <summary>
        /// Search payments by transaction code (Admin/Staff only)
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SearchPayments([FromQuery] string transactionCode)
        {
            if (string.IsNullOrWhiteSpace(transactionCode))
                return BadRequest(new { message = "Transaction code is required" });

            var payments = await _paymentService.SearchPaymentsByTransactionCodeAsync(transactionCode);
            return Ok(payments);
        }

        /// <summary>
        /// Get payments by date range (Admin/Staff only)
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetPaymentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest(new { message = "Start date must be before end date" });

            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return Ok(payments);
        }

        /// <summary>
        /// Get total revenue (Admin only)
        /// </summary>
        [HttpGet("revenue/total")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var totalRevenue = await _paymentService.GetTotalRevenueAsync();
            return Ok(new { totalRevenue });
        }

        /// <summary>
        /// Get payment statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPaymentStatistics()
        {
            var statistics = await _paymentService.GetPaymentStatisticsAsync();
            return Ok(statistics);
        }

        /// <summary>
        /// Verify payment with transaction code (webhook endpoint or manual verification)
        /// </summary>
        [HttpPost("{id:int}/verify")]
        [AllowAnonymous] // Can be restricted based on webhook signature
        public async Task<IActionResult> VerifyPayment(int id, [FromBody] VerifyPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _paymentService.VerifyPaymentAsync(id, dto.TransactionCode);
            if (success)
                return Ok(new { message = "Payment verified successfully" });

            return BadRequest(new { message = "Payment verification failed" });
        }
    }

    /// <summary>
    /// DTO for verifying payment
    /// </summary>
    public class VerifyPaymentDto
    {
        [Required]
        public string TransactionCode { get; set; } = string.Empty;
    }
}
