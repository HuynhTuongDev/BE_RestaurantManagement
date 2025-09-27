namespace RestaurantManagement.Domain.Entities
{
    public enum TableStatus
    {
        Available,
        Occupied,
        Reserved
    }

    //Reserved=2,Available=0,Occupied=1

    public class RestaurantTable
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public int Seats { get; set; }
        public TableStatus Status { get; set; } = TableStatus.Available;
        public string? Location { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
