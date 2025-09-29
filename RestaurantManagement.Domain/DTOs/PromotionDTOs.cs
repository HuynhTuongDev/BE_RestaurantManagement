using System;

namespace RestaurantManagement.Domain.DTOs
{
    public class PromotionCreateDto
    {
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Discount { get; set; } // ví d? 10 = 10%
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }

    }

    public class PromotionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Discount { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
