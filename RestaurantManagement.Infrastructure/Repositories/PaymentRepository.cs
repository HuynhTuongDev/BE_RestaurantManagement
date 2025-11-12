using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Infrastructure.Data;
using RestaurantManagement.Infrastructure.Repositories.Base;

namespace RestaurantManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Payment repository implementation
    /// </summary>
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(RestaurantDbContext context, ILogger<PaymentRepository> logger)
            : base(context, logger)
        {
        }

        /// <summary>
        /// Create payment (override for additional logic)
        /// </summary>
        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            try
            {
                Logger.LogInformation("Creating Payment for Order {OrderId}", payment.OrderId);
                return await CreateAsync(payment);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error creating Payment for Order {OrderId}", payment.OrderId);
                throw;
            }
        }

        /// <summary>
        /// Get payment by id with all details
        /// </summary>
        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                Logger.LogInformation("Getting Payment {PaymentId} with details", paymentId);
                
                return await DbSet
                    .Include(p => p.PaymentDetails)
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.Id == paymentId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Payment {PaymentId}", paymentId);
                throw;
            }
        }

        /// <summary>
        /// Get all payments for an order
        /// </summary>
        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            try
            {
                Logger.LogInformation("Getting Payments for Order {OrderId}", orderId);
                
                return await DbSet
                    .Where(p => p.OrderId == orderId)
                    .Include(p => p.PaymentDetails)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Payments for Order {OrderId}", orderId);
                throw;
            }
        }

        /// <summary>
        /// Get all payments with details
        /// </summary>
        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            try
            {
                Logger.LogInformation("Getting all Payments with details");
                
                return await DbSet
                    .Include(p => p.PaymentDetails)
                    .Include(p => p.Order)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting all Payments");
                throw;
            }
        }

        /// <summary>
        /// Get payments by status
        /// </summary>
        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            try
            {
                Logger.LogInformation("Getting Payments with status: {Status}", status);
                
                return await DbSet
                    .Where(p => p.Status == status)
                    .Include(p => p.PaymentDetails)
                    .Include(p => p.Order)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Payments with status: {Status}", status);
                throw;
            }
        }

        /// <summary>
        /// Update payment status
        /// </summary>
        public async Task<Payment?> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status)
        {
            try
            {
                Logger.LogInformation("Updating Payment {PaymentId} status to {Status}", paymentId, status);
                
                var payment = await DbSet.FirstOrDefaultAsync(p => p.Id == paymentId);
                if (payment == null)
                {
                    Logger.LogWarning("Payment {PaymentId} not found", paymentId);
                    return null;
                }

                payment.Status = status;
                
                await Context.SaveChangesAsync();
                Logger.LogInformation("Successfully updated Payment {PaymentId} status to {Status}", paymentId, status);
                
                return payment;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error updating Payment {PaymentId} status", paymentId);
                throw;
            }
        }

        /// <summary>
        /// Delete payment
        /// </summary>
        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            try
            {
                Logger.LogInformation("Deleting Payment {PaymentId}", paymentId);
                return await DeleteAsync(paymentId);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error deleting Payment {PaymentId}", paymentId);
                throw;
            }
        }

        /// <summary>
        /// Search payments by transaction code
        /// </summary>
        public async Task<IEnumerable<Payment>> SearchPaymentsByTransactionCodeAsync(string transactionCode)
        {
            try
            {
                Logger.LogInformation("Searching Payments with transaction code: {TransactionCode}", transactionCode);
                
                if (string.IsNullOrWhiteSpace(transactionCode))
                {
                    Logger.LogWarning("Transaction code is empty");
                    return new List<Payment>();
                }

                return await DbSet
                    .Where(p => p.PaymentDetails != null && 
                        p.PaymentDetails.Any(pd => pd.TransactionCode != null && 
                            pd.TransactionCode.Contains(transactionCode)))
                    .Include(p => p.PaymentDetails)
                    .Include(p => p.Order)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error searching Payments with transaction code: {TransactionCode}", transactionCode);
                throw;
            }
        }

        /// <summary>
        /// Get payments by date range
        /// </summary>
        public async Task<IEnumerable<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                Logger.LogInformation("Getting Payments between {StartDate} and {EndDate}", startDate, endDate);
                
                return await DbSet
                    .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                    .Include(p => p.PaymentDetails)
                    .Include(p => p.Order)
                    .OrderByDescending(p => p.PaymentDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error getting Payments between {StartDate} and {EndDate}", startDate, endDate);
                throw;
            }
        }

        /// <summary>
        /// Get total payment amount by status
        /// </summary>
        public async Task<decimal> GetTotalPaymentsByStatusAsync(PaymentStatus status)
        {
            try
            {
                Logger.LogInformation("Calculating total payments with status: {Status}", status);
                
                return await DbSet
                    .Where(p => p.Status == status)
                    .SumAsync(p => p.Amount);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error calculating total payments with status: {Status}", status);
                throw;
            }
        }

        /// <summary>
        /// Get payment count by status
        /// </summary>
        public async Task<int> GetPaymentCountByStatusAsync(PaymentStatus status)
        {
            try
            {
                Logger.LogInformation("Counting payments with status: {Status}", status);
                
                return await DbSet
                    .Where(p => p.Status == status)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error counting payments with status: {Status}", status);
                throw;
            }
        }
    }
}
