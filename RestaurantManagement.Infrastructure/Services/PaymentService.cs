using RestaurantManagement.Domain.DTOs;
using RestaurantManagement.Domain.DTOs.Common;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Interfaces;
using RestaurantManagement.Application.Services;

namespace RestaurantManagement.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(PaymentCreateDto dto)
        {
            try
            {
                // Validate order exists
                var order = await _orderRepository.GetByIdAsync(dto.OrderId);
                if (order == null)
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Order not found"
                    };

                // Validate amount matches order total
                if (dto.Amount <= 0)
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment amount must be greater than 0"
                    };

                // Create payment entity
                var payment = new Payment
                {
                    OrderId = dto.OrderId,
                    Amount = dto.Amount,
                    Status = PaymentStatus.Pending,
                    PaymentDate = DateTime.UtcNow,
                    PaymentDetails = new List<PaymentDetail>()
                };

                // Add payment details
                foreach (var detail in dto.PaymentDetails)
                {
                    payment.PaymentDetails.Add(new PaymentDetail
                    {
                        Method = detail.Method,
                        Amount = detail.Amount,
                        TransactionCode = detail.TransactionCode,
                        Provider = detail.Provider,
                        ExtraInfo = detail.ExtraInfo
                    });
                }

                // Save to database
                var createdPayment = await _paymentRepository.CreatePaymentAsync(payment);

                return new PaymentResponse
                {
                    Success = true,
                    Message = "Payment created successfully",
                    Payment = MapToPaymentDto(createdPayment)
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error creating payment: {ex.Message}"
                };
            }
        }

        public async Task<PaymentDto?> GetPaymentByIdAsync(int paymentId)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            return payment != null ? MapToPaymentDto(payment) : null;
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByOrderIdAsync(int orderId)
        {
            var payments = await _paymentRepository.GetPaymentsByOrderIdAsync(orderId);
            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<PaymentListResponse> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _paymentRepository.GetAllPaymentsAsync();
                return new PaymentListResponse
                {
                    Success = true,
                    Message = "Payments retrieved successfully",
                    Payments = payments.Select(MapToPaymentDto).ToList()
                };
            }
            catch (Exception ex)
            {
                return new PaymentListResponse
                {
                    Success = false,
                    Message = $"Error retrieving payments: {ex.Message}",
                    Payments = new List<PaymentDto>()
                };
            }
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            var payments = await _paymentRepository.GetPaymentsByStatusAsync(status);
            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<PaymentResponse> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status)
        {
            try
            {
                var payment = await _paymentRepository.UpdatePaymentStatusAsync(paymentId, status);
                if (payment == null)
                    return new PaymentResponse
                    {
                        Success = false,
                        Message = "Payment not found"
                    };

                // If payment is completed, update order status
                if (status == PaymentStatus.Completed && payment.Order != null)
                {
                    // Order status update logic can be added here
                }

                return new PaymentResponse
                {
                    Success = true,
                    Message = "Payment status updated successfully",
                    Payment = MapToPaymentDto(payment)
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Success = false,
                    Message = $"Error updating payment: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeletePaymentAsync(int paymentId)
        {
            return await _paymentRepository.DeletePaymentAsync(paymentId);
        }

        public async Task<IEnumerable<PaymentDto>> SearchPaymentsByTransactionCodeAsync(string transactionCode)
        {
            if (string.IsNullOrWhiteSpace(transactionCode))
                return new List<PaymentDto>();

            var payments = await _paymentRepository.SearchPaymentsByTransactionCodeAsync(transactionCode);
            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepository.GetPaymentsByDateRangeAsync(startDate, endDate);
            return payments.Select(MapToPaymentDto).ToList();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _paymentRepository.GetTotalPaymentsByStatusAsync(PaymentStatus.Completed);
        }

        public async Task<PaymentStatisticsDto> GetPaymentStatisticsAsync()
        {
            var completedAmount = await _paymentRepository.GetTotalPaymentsByStatusAsync(PaymentStatus.Completed);
            var pendingAmount = await _paymentRepository.GetTotalPaymentsByStatusAsync(PaymentStatus.Pending);
            var failedAmount = await _paymentRepository.GetTotalPaymentsByStatusAsync(PaymentStatus.Failed);

            var completedCount = await _paymentRepository.GetPaymentCountByStatusAsync(PaymentStatus.Completed);
            var pendingCount = await _paymentRepository.GetPaymentCountByStatusAsync(PaymentStatus.Pending);
            var failedCount = await _paymentRepository.GetPaymentCountByStatusAsync(PaymentStatus.Failed);

            return new PaymentStatisticsDto
            {
                TotalCompleted = completedAmount,
                TotalPending = pendingAmount,
                TotalFailed = failedAmount,
                CountCompleted = completedCount,
                CountPending = pendingCount,
                CountFailed = failedCount,
                TotalRevenue = completedAmount
            };
        }

        public async Task<bool> VerifyPaymentAsync(int paymentId, string transactionCode)
        {
            var payment = await _paymentRepository.GetPaymentByIdAsync(paymentId);
            if (payment == null)
                return false;

            // Verify transaction code matches
            var matchingDetail = payment.PaymentDetails
                .FirstOrDefault(pd => pd.TransactionCode == transactionCode);

            if (matchingDetail == null)
                return false;

            // Update payment status to completed if verified
            await _paymentRepository.UpdatePaymentStatusAsync(paymentId, PaymentStatus.Completed);
            return true;
        }

        public async Task<PaginatedResponse<PaymentDto>> GetPaginatedAsync(PaginationRequest pagination)
        {
            var allPayments = await _paymentRepository.GetAllPaymentsAsync();
            
            var totalCount = allPayments.Count();
            var paginatedData = allPayments
                .Skip(pagination.SkipCount)
                .Take(pagination.PageSize)
                .Select(MapToPaymentDto)
                .ToList();

            return PaginatedResponse<PaymentDto>.Create(
                paginatedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalCount);
        }

        public async Task<PaginatedResponse<PaymentDto>> SearchPaginatedAsync(
            string transactionCode,
            PaginationRequest pagination)
        {
            if (string.IsNullOrWhiteSpace(transactionCode))
            {
                return PaginatedResponse<PaymentDto>.Create(
                    new List<PaymentDto>(),
                    pagination.PageNumber,
                    pagination.PageSize,
                    0);
            }

            var payments = await _paymentRepository.SearchPaymentsByTransactionCodeAsync(transactionCode);

            var totalCount = payments.Count();
            var paginatedData = payments
                .Skip(pagination.SkipCount)
                .Take(pagination.PageSize)
                .Select(MapToPaymentDto)
                .ToList();

            return PaginatedResponse<PaymentDto>.Create(
                paginatedData,
                pagination.PageNumber,
                pagination.PageSize,
                totalCount);
        }

        // Helper method
        private PaymentDto MapToPaymentDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                PaymentDate = payment.PaymentDate,
                PaymentDetails = payment.PaymentDetails.Select(pd => new PaymentDetailDto
                {
                    Id = pd.Id,
                    PaymentId = pd.PaymentId,
                    Method = pd.Method.ToString(),
                    Amount = pd.Amount,
                    TransactionCode = pd.TransactionCode,
                    Provider = pd.Provider,
                    ExtraInfo = pd.ExtraInfo
                }).ToList()
            };
        }
    }
}
