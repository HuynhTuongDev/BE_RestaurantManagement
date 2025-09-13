namespace RestaurantManagement.Domain.Entities
{
    public enum MenuItemStatus
    {
        Available,
        OutOfStock
    }

    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public MenuItemStatus Status { get; set; } = MenuItemStatus.Available;

        public ICollection<MenuItemImage> Images { get; set; } = new List<MenuItemImage>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
