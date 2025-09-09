using Microsoft.EntityFrameworkCore;

namespace RestaurantManagement.Infrastructure.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

    }
}
