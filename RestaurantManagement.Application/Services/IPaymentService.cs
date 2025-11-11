using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
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

        Task<PaginatedResponse<PaymentDto>> GetPaginatedAsync(PaginationRequest pagination);

        Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(PaymentStatus status);

        // Update payment status
        Task<PaymentResponse> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);

        // Delete payment
        Task<bool> DeletePaymentAsync(int paymentId);

        // Search and analytics
        Task<IEnumerable<PaymentDto>> SearchPaymentsByTransactionCodeAsync(string transactionCode);

        Task<PaginatedResponse<PaymentDto>> SearchPaginatedAsync(string transactionCode, PaginationRequest pagination);

        Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<decimal> GetTotalRevenueAsync();

        Task<PaymentStatisticsDto> GetPaymentStatisticsAsync();

        // Verify payment (webhook support)
        Task<bool> VerifyPaymentAsync(int paymentId, string transactionCode);
    }

}
