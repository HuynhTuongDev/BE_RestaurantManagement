using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;

namespace RestaurantManagement.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly RestaurantDbContext _context;

        public PaymentRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.PaymentDetails)
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            return await _context.Payments
                .Where(p => p.OrderId == orderId)
                .Include(p => p.PaymentDetails)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.PaymentDetails)
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .Include(p => p.PaymentDetails)
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<Payment?> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
            if (payment == null)
                return null;

            payment.Status = status;
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);
            if (payment == null)
                return false;

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Payment>> SearchPaymentsByTransactionCodeAsync(string transactionCode)
        {
            return await _context.Payments
                .Where(p => p.PaymentDetails != null && p.PaymentDetails.Any(pd => pd.TransactionCode != null && pd.TransactionCode.Contains(transactionCode)))
                .Include(p => p.PaymentDetails)
                .Include(p => p.Order)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .Include(p => p.PaymentDetails)
                .Include(p => p.Order)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .SumAsync(p => p.Amount);
        }

        public async Task<int> GetPaymentCountByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .CountAsync();
        }
    }
}
