namespace RestaurantManagement.Domain.Entities
{
    namespace RestaurantManagement.Domain.Entities
    {
        public class Feedback
        {
            public int Id { get; set; }
            public int UserId { get; set; }
            public User User { get; set; } = null!;
            
            public int? OrderId { get; set; }
            public Order? Order { get; set; }

            public int? MenuItemId { get; set; }
            public MenuItem? MenuItem { get; set; }
            
            public int Rating { get; set; } // 1-5 stars
            public string? Comment { get; set; }
            public bool IsApproved { get; set; } = false;
            
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
            
            public string? Reply { get; set; }
            public DateTime? RepliedAt { get; set; }
        }
    }

}
