using Microsoft.EntityFrameworkCore;
using RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Infrastructure.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<StaffProfile> StaffProfiles { get; set; } = null!;
        public DbSet<RestaurantTable> RestaurantTables { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Promotion> Promotions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.FullName).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
                entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
                entity.Property(u => u.Role).IsRequired(); // Enum int
                entity.Property(u => u.CreatedAt).IsRequired();

                // Unique constraints
                entity.HasIndex(u => u.Email).IsUnique();

                // Indexes for performance
                entity.HasIndex(u => u.Role);
                entity.HasIndex(u => u.CreatedAt);
            });

            // StaffProfile Configuration
            modelBuilder.Entity<StaffProfile>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Position).IsRequired().HasMaxLength(100);
                entity.Property(s => s.HireDate).IsRequired();

                // Index on UserId for performance
                entity.HasIndex(s => s.UserId).IsUnique();
            });

            // RestaurantTable Configuration
            modelBuilder.Entity<RestaurantTable>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TableNumber).IsRequired();
                entity.Property(t => t.Seats).IsRequired();
                entity.Property(t => t.Status).IsRequired(); // Enum int
                entity.Property(t => t.Location).HasMaxLength(255);

                // Unique constraint for table number
                entity.HasIndex(t => t.TableNumber).IsUnique();

                // Index for performance
                entity.HasIndex(t => t.Status);
            });

            // MenuItem Configuration
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(m => m.Id);
                entity.Property(m => m.Name).IsRequired().HasMaxLength(255);
                entity.Property(m => m.Description).HasMaxLength(1000);
                entity.Property(m => m.Price).HasPrecision(10, 2).IsRequired();
                entity.Property(m => m.Category).HasMaxLength(100);
                entity.Property(m => m.Status).IsRequired(); // Enum int

                // Indexes for performance
                entity.HasIndex(m => m.Category);
                entity.HasIndex(m => m.Status);
                entity.HasIndex(m => m.Name);
            });

            // Reservation Configuration
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(r => r.Id);
                entity.Property(r => r.UserId).IsRequired();
                entity.Property(r => r.TableId).IsRequired();
                entity.Property(r => r.ReservationTime).IsRequired();
                entity.Property(r => r.NumberOfGuests).IsRequired();
                entity.Property(r => r.Status).IsRequired(); // Enum int

                // Indexes for performance
                entity.HasIndex(r => r.ReservationTime);
                entity.HasIndex(r => r.Status);
                entity.HasIndex(r => new { r.TableId, r.ReservationTime });
            });

            // Order Configuration
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.UserId).IsRequired();
                entity.Property(o => o.TableId).IsRequired();
                entity.Property(o => o.OrderTime).IsRequired();
                entity.Property(o => o.Status).IsRequired(); // Enum int
                entity.Property(o => o.TotalAmount).HasPrecision(10, 2).IsRequired();

                // Indexes for performance
                entity.HasIndex(o => o.OrderTime);
                entity.HasIndex(o => o.Status);
                entity.HasIndex(o => new { o.TableId, o.OrderTime });
            });

            // OrderDetail Configuration
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.Id);
                entity.Property(od => od.OrderId).IsRequired();
                entity.Property(od => od.MenuItemId).IsRequired();
                entity.Property(od => od.Quantity).IsRequired();
                entity.Property(od => od.Price).HasPrecision(10, 2).IsRequired();

                // Composite index for performance
                entity.HasIndex(od => new { od.OrderId, od.MenuItemId });
            });

            // Promotion Configuration
            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Code).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.Property(p => p.Discount).HasPrecision(5, 2).IsRequired();
                entity.Property(p => p.StartDate).IsRequired();
                entity.Property(p => p.EndDate).IsRequired();
                entity.Property(p => p.Status).IsRequired(); // Enum int

                // Unique constraint for promotion code
                entity.HasIndex(p => p.Code).IsUnique();

                // Indexes for performance
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => new { p.StartDate, p.EndDate });
            });

            // 1-1 relationship User <-> StaffProfile
            modelBuilder.Entity<User>()
                .HasOne(u => u.StaffProfile)
                .WithOne(s => s.User)
                .HasForeignKey<StaffProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-n relationships
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestaurantTable>()
                .HasMany(t => t.Reservations)
                .WithOne(r => r.Table)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RestaurantTable>()
                .HasMany(t => t.Orders)
                .WithOne(o => o.Table)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MenuItem>()
                .HasMany(m => m.OrderDetails)
                .WithOne(od => od.MenuItem)
                .HasForeignKey(od => od.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
