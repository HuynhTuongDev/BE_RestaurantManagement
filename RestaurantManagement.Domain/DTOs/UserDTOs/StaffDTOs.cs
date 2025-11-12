using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Domain.DTOs.UserDTOs
{
    public class StaffCreateRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MinLength(6)]
        public string Password { get; set; } = "12345678"; // Default password

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string Position { get; set; } = string.Empty; // Waiter, Chef, Manager

        public DateTime HireDate { get; set; } = DateTime.UtcNow;
    }

    public class StaffResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StaffDto? Staff { get; set; }
    }

    public class StaffDto : UserDto
    {
        // Inherits Id, Email, FullName, Phone, Address, Role, Status, CreatedAt, UpdatedAt, IsDeleted
        public string Position { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
    }
}
