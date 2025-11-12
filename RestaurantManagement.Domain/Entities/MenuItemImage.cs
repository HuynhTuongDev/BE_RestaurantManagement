using System.Text.Json.Serialization;

namespace RestaurantManagement.Domain.Entities
{
    public class MenuItemImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;

        public int MenuItemId { get; set; }
        
        [JsonIgnore] // Prevent circular reference when serializing
        public MenuItem MenuItem { get; set; } = null!;
    }
}
