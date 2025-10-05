using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Domain.DTOs
{
    public class PromotionCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; } = null!;

        [StringLength(200)]
        public string? Description { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100")]
        public decimal Discount { get; set; } // ví dụ 10 = 10%

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class PromotionDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
