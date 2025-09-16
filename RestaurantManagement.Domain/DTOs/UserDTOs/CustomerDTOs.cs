using System.ComponentModel.DataAnnotations;

namespace RestaurantManagement.Domain.DTOs.UserDTOs
{
    public class CustomerCreateRequest
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; } // Optional for walk-in customers

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }
    }

    public class CustomerResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public CustomerDto? Customer { get; set; }
    }

    public class CustomerDto : UserDto
    {
        // Inherits Id, Email, FullName, Phone, Address, Role, Status, CreatedAt, UpdatedAt, IsDeleted
    }
}
