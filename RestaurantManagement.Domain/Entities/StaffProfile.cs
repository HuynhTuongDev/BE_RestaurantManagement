namespace RestaurantManagement.Domain.Entities
{
    public class StaffProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Position { get; set; } = null!; // Waiter, Chef, Manager
        public DateTime HireDate { get; set; }

        public User User { get; set; } = null!;
    }

}
