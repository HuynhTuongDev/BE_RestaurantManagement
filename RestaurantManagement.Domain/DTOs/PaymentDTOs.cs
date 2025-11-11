using RestaurantManagement.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Domain.DTOs
{
    /// <summary>
    /// DTO for creating payment details
    /// </summary>
    public class PaymentDetailCreateDto
    {
        [Required]
        public PaymentMethod Method { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? TransactionCode { get; set; }

        public string? Provider { get; set; }

        public string? ExtraInfo { get; set; }
    }

    /// <summary>
    /// DTO for creating a payment
    /// </summary>
    public class PaymentCreateDto
    {
        [Required]
        public int OrderId { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        [MinLength(1)]
        public List<PaymentDetailCreateDto> PaymentDetails { get; set; } = new();
    }

    /// <summary>
    /// DTO for payment details response
    /// </summary>
    public class PaymentDetailDto
    {
        public int Id { get; set; }

        public int PaymentId { get; set; }

        public string Method { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string? TransactionCode { get; set; }

        public string? Provider { get; set; }

        public string? ExtraInfo { get; set; }
    }

    /// <summary>
    /// DTO for payment response
    /// </summary>
    public class PaymentDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime PaymentDate { get; set; }

        public List<PaymentDetailDto> PaymentDetails { get; set; } = new();
    }

    /// <summary>
    /// DTO for API response
    /// </summary>
    public class PaymentResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public PaymentDto? Payment { get; set; }
    }

    /// <summary>
    /// DTO for listing payments
    /// </summary>
    public class PaymentListResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public IEnumerable<PaymentDto> Payments { get; set; } = new List<PaymentDto>();
    }

    /// <summary>
    /// DTO for payment status update
    /// </summary>
    public class PaymentStatusUpdateDto
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }
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
