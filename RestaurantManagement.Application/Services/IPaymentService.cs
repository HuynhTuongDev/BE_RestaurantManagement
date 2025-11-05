using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Application.Services
{
    public interface IPaymentService
    {
        // Create payment
        Task<PaymentResponse> CreatePaymentAsync(PaymentCreateDto dto);

        // Get payments
        Task<PaymentDto?> GetPaymentByIdAsync(int paymentId);

        Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(int orderId);

        Task<PaymentListResponse> GetAllPaymentsAsync();

        Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(PaymentStatus status);

        // Update payment status
        Task<PaymentResponse> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);

        // Delete payment
        Task<bool> DeletePaymentAsync(int paymentId);

        // Search and analytics
        Task<IEnumerable<PaymentDto>> SearchPaymentsByTransactionCodeAsync(string transactionCode);

        Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<decimal> GetTotalRevenueAsync();

        Task<PaymentStatisticsDto> GetPaymentStatisticsAsync();

        // Verify payment (webhook support)
        Task<bool> VerifyPaymentAsync(int paymentId, string transactionCode);
    }

    /// <summary>
    /// DTO for payment statistics
    /// </summary>
    public class PaymentStatisticsDto
    {
        public decimal TotalCompleted { get; set; }

        public decimal TotalPending { get; set; }

        public decimal TotalFailed { get; set; }

        public int CountCompleted { get; set; }

        public int CountPending { get; set; }

        public int CountFailed { get; set; }

        public decimal TotalRevenue { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
