namespace RestaurantManagement.Domain.DTOs.UserDTOs
{
    public class FeedbackDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public int? OrderId { get; set; }

        public int? MenuItemId { get; set; }
        public string? MenuItemName { get; set; }

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public bool IsApproved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? Reply { get; set; }
        public DateTime? RepliedAt { get; set; }
     
    }
    public class CreateFeedbackDto
    {
        public int UserId { get; set; }
        public int? OrderId { get; set; }
        public int? MenuItemId { get; set; }

        public int Rating { get; set; }
        public bool IsApproved { get; set; }
        public string? Comment { get; set; }
    }
}
