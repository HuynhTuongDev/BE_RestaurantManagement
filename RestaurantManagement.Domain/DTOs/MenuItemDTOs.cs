using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantManagement.Domain.DTOs
{
    public class MenuItemCreateDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for MenuItem response (without circular references)
    /// </summary>
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<MenuItemImageDto> Images { get; set; } = new();
    }

    /// <summary>
    /// DTO for MenuItemImage response (without circular references)
    /// </summary>
    public class MenuItemImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int MenuItemId { get; set; }
        // Note: NO MenuItem property to avoid circular reference
    }
}
