namespace RestaurantManagement.Domain.Entities
{
    public enum PromotionStatus
    {
        Active,
        Expired
    }

    public class Promotion
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Discount { get; set; } // 10 = 10%
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public PromotionStatus Status { get; set; } = PromotionStatus.Active;
    }

}
