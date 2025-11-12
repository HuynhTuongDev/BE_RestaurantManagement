using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantManagement.Api.Controllers.Base;
using RestaurantManagement.Application.Services;
using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Api.Controllers
{
    [Route("api/payment")]
    [ApiVersion("1.0")]
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
            : base(logger)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Create a new payment for an order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid payment data");

            var result = await _paymentService.CreatePaymentAsync(dto);
            if (result.Success)
                return CreatedResponse(nameof(GetPaymentById), result.Payment!.Id, result.Payment, result.Message);

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Get all payments (Admin/Staff only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPayments()
        {
            var result = await _paymentService.GetAllPaymentsAsync();
            return OkListResponse(result.Payments, result.Message);
        }

        /// <summary>
        /// Get paginated payments (Admin/Staff only)
        /// </summary>
        [HttpGet("paginated")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaginatedPayments([FromQuery] PaginationRequest pagination)
        {
            var paginatedPayments = await _paymentService.GetPaginatedAsync(pagination);
            return OkPaginatedResponse(paginatedPayments, "Payments retrieved successfully");
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFoundResponse("Payment not found");

            return OkResponse(payment, "Payment retrieved successfully");
        }

        /// <summary>
        /// Get all payments for a specific order
        /// </summary>
        [HttpGet("order/{orderId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentsByOrderId(int orderId)
        {
            var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
            return OkListResponse(payments, "Payments retrieved successfully");
        }

        /// <summary>
        /// Get payments by status (Admin/Staff only)
        /// </summary>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentsByStatus(PaymentStatus status)
        {
            var payments = await _paymentService.GetPaymentsByStatusAsync(status);
            return OkListResponse(payments, "Payments retrieved successfully");
        }

        /// <summary>
        /// Update payment status (Admin only)
        /// </summary>
        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePaymentStatus(int id, [FromBody] PaymentStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid status update data");

            var result = await _paymentService.UpdatePaymentStatusAsync(id, dto.Status);
            if (result.Success)
                return OkResponse(result.Payment!, result.Message);

            return BadRequestResponse(result.Message);
        }

        /// <summary>
        /// Delete payment (Admin only)
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePayment(int id)
        {
            var success = await _paymentService.DeletePaymentAsync(id);
            if (success)
                return OkResponse(new { deleted = true }, "Payment deleted successfully");

            return NotFoundResponse("Payment not found");
        }

        /// <summary>
        /// Search payments by transaction code (Admin/Staff only)
        /// </summary>
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPayments([FromQuery] string transactionCode)
        {
            if (string.IsNullOrWhiteSpace(transactionCode))
                return BadRequestResponse("Transaction code is required");

            var payments = await _paymentService.SearchPaymentsByTransactionCodeAsync(transactionCode);
            return OkListResponse(payments, "Search completed successfully");
        }

        /// <summary>
        /// Search payments by transaction code with pagination (Admin/Staff only)
        /// </summary>
        [HttpGet("search/paginated")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SearchPaginatedPayments(
            [FromQuery] string transactionCode,
            [FromQuery] PaginationRequest pagination)
        {
            if (string.IsNullOrWhiteSpace(transactionCode))
                return BadRequestResponse("Transaction code is required");

            var paginatedPayments = await _paymentService.SearchPaginatedAsync(transactionCode, pagination);
            return OkPaginatedResponse(paginatedPayments, "Search completed successfully");
        }

        /// <summary>
        /// Get payments by date range (Admin/Staff only)
        /// </summary>
        [HttpGet("date-range")]
        [Authorize(Roles = "Admin,Staff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPaymentsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequestResponse("Start date must be before end date");

            var payments = await _paymentService.GetPaymentsByDateRangeAsync(startDate, endDate);
            return OkListResponse(payments, "Payments retrieved successfully");
        }

        /// <summary>
        /// Get total revenue (Admin only)
        /// </summary>
        [HttpGet("revenue/total")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var totalRevenue = await _paymentService.GetTotalRevenueAsync();
            return OkResponse(new { totalRevenue }, "Total revenue retrieved successfully");
        }

        /// <summary>
        /// Get payment statistics (Admin only)
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentStatistics()
        {
            var statistics = await _paymentService.GetPaymentStatisticsAsync();
            return OkResponse(statistics, "Statistics retrieved successfully");
        }

        /// <summary>
        /// Verify payment with transaction code (webhook endpoint or manual verification)
        /// </summary>
        [HttpPost("{id:int}/verify")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> VerifyPayment(int id, [FromBody] VerifyPaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestResponse("Invalid verification data");

            var success = await _paymentService.VerifyPaymentAsync(id, dto.TransactionCode);
            if (success)
                return OkResponse(new { verified = true }, "Payment verified successfully");

            return BadRequestResponse("Payment verification failed");
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
