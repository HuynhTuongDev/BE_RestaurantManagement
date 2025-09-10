using RestaurantManagement.Domain.Entities.RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Domain.Entities
{
    public enum UserRole
    {
        Admin,
        Staff,
        Customer
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended
    }

    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string? Phone { get; set; }
        public string? Address { get; set; }

        public UserRole Role { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public StaffProfile? StaffProfile { get; set; }
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }

}
