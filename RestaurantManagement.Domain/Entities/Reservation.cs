namespace RestaurantManagement.Domain.Entities
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }

    public class Reservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TableId { get; set; }
        public DateTime ReservationTime { get; set; }
        public int NumberOfGuests { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public User User { get; set; } = null!;
        public RestaurantTable Table { get; set; } = null!;
    }

}
