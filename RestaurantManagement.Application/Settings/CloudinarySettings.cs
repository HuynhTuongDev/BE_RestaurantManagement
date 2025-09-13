namespace RestaurantManagement.Application.Settings
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecret { get; set; } = string.Empty;
        public string Folder { get; set; } = "restaurant/menuitems";
        public int MaxFileSizeKb { get; set; } = 5120; // 5MB default
        public string[] AllowedFormats { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    }
}