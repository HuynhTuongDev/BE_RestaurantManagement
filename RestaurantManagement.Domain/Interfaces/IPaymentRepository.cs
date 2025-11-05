using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Interfaces
{
    public interface IPaymentRepository
    {
        // Create
        Task<Payment> CreatePaymentAsync(Payment payment);

        // Read
        Task<Payment?> GetPaymentByIdAsync(int paymentId);

        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId);

        Task<IEnumerable<Payment>> GetAllPaymentsAsync();

        Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);

        // Update
        Task<Payment?> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);

        // Delete
        Task<bool> DeletePaymentAsync(int paymentId);

        // Search and Filter
        Task<IEnumerable<Payment>> SearchPaymentsByTransactionCodeAsync(string transactionCode);

        Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<decimal> GetTotalPaymentsByStatusAsync(PaymentStatus status);

        Task<int> GetPaymentCountByStatusAsync(PaymentStatus status);
    }
}
