using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RestaurantManagement.Domain.Entities;
using RestaurantManagement.Domain.Entities.RestaurantManagement.Domain.Entities;

namespace RestaurantManagement.Infrastructure.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<StaffProfile> StaffProfiles { get; set; } = null!;
        public DbSet<RestaurantTable> RestaurantTables { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        public DbSet<MenuItemImage> MenuItemImages { get; set; } = null!;
        public DbSet<Reservation> Reservations { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Promotion> Promotions { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<PaymentDetail> PaymentDetails { get; set; } = null!;

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
                entity.Property(u => u.Phone).HasMaxLength(20);
                entity.Property(u => u.Address).HasMaxLength(500);
                entity.Property(u => u.Role).IsRequired(); // Enum int
                entity.Property(u => u.Status).IsRequired();
                entity.Property(u => u.CreatedAt).IsRequired();
                entity.Property(u => u.UpdatedAt);
                entity.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);

                // Unique constraints
                entity.HasIndex(u => u.Email).IsUnique();

                // Indexes for performance
                entity.HasIndex(u => u.Role);
                entity.HasIndex(u => u.Status);
                entity.HasIndex(u => u.CreatedAt);
                entity.HasIndex(u => u.Phone);
                entity.HasIndex(u => u.IsDeleted);
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

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_RestaurantTable_Seats", "\"Seats\" > 0")
                );

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

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_MenuItem_Price", "\"Price\" >= 0")
                );

                // Indexes for performance
                entity.HasIndex(m => m.Category);
                entity.HasIndex(m => m.Status);
                entity.HasIndex(m => m.Name);
            });
            // MenuItemImage Configuration
            modelBuilder.Entity<MenuItemImage>(entity =>
            {
                entity.HasKey(mi => mi.Id);
                entity.Property(mi => mi.ImageUrl).IsRequired().HasMaxLength(1000);

                entity.HasIndex(mi => mi.MenuItemId);

                // Quan hệ 1-nhiều
                entity.HasOne(mi => mi.MenuItem)
                      .WithMany(m => m.Images)
                      .HasForeignKey(mi => mi.MenuItemId)
                      .OnDelete(DeleteBehavior.Cascade);
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

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_Reservation_NumberOfGuests", "\"NumberOfGuests\" > 0")
                );

                // Indexes for performance
                entity.HasIndex(r => r.ReservationTime);
                entity.HasIndex(r => r.Status);
                entity.HasIndex(r => new { r.TableId, r.ReservationTime });
                entity.HasIndex(r => new { r.UserId, r.ReservationTime });
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

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_Order_TotalAmount", "\"TotalAmount\" >= 0")
                );

                // Indexes for performance
                entity.HasIndex(o => o.OrderTime);
                entity.HasIndex(o => o.Status);
                entity.HasIndex(o => new { o.TableId, o.OrderTime });
                entity.HasIndex(o => new { o.UserId, o.OrderTime });
            });

            // OrderDetail Configuration
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(od => od.Id);
                entity.Property(od => od.OrderId).IsRequired();
                entity.Property(od => od.MenuItemId).IsRequired();
                entity.Property(od => od.Quantity).IsRequired();
                entity.Property(od => od.Price).HasPrecision(10, 2).IsRequired();

                // Business constraints
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_OrderDetail_Quantity", "\"Quantity\" > 0");
                    tb.HasCheckConstraint("CK_OrderDetail_Price", "\"Price\" >= 0");
                });

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

                // Business constraints
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Promotion_Discount", "\"Discount\" >= 0 AND \"Discount\" <= 100");
                    tb.HasCheckConstraint("CK_Promotion_Dates", "\"EndDate\" > \"StartDate\"");
                });

                // Unique constraint for promotion code
                entity.HasIndex(p => p.Code).IsUnique();

                // Indexes for performance
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => new { p.StartDate, p.EndDate });
            });

            // Feedback Configuration
            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.UserId).IsRequired();
                entity.Property(f => f.CreatedAt).IsRequired();
                entity.Property(f => f.Rating).IsRequired();
                entity.Property(f => f.Comment).HasMaxLength(2000);
                entity.Property(f => f.IsApproved).IsRequired().HasDefaultValue(false);
                entity.Property(f => f.UpdatedAt);
                entity.Property(f => f.Reply).HasMaxLength(2000);
                entity.Property(f => f.RepliedAt);

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_Feedback_Rating", "\"Rating\" >= 1 AND \"Rating\" <= 5")
                );

                // Indexes for performance  
                entity.HasIndex(f => f.UserId);
                entity.HasIndex(f => f.CreatedAt);
                entity.HasIndex(f => f.IsApproved);
                entity.HasIndex(f => f.Rating);
                entity.HasIndex(f => f.OrderId);
                entity.HasIndex(f => f.MenuItemId);
                entity.HasIndex(f => new { f.UserId, f.CreatedAt });
            });

            // Payment Configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.OrderId).IsRequired();
                entity.Property(p => p.Amount).HasPrecision(10, 2).IsRequired();
                entity.Property(p => p.Status).IsRequired(); // Enum int
                entity.Property(p => p.PaymentDate).IsRequired();

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_Payment_Amount", "\"Amount\" > 0")
                );

                // Indexes for performance
                entity.HasIndex(p => p.OrderId);
                entity.HasIndex(p => p.Status);
                entity.HasIndex(p => p.PaymentDate);
                entity.HasIndex(p => new { p.Status, p.PaymentDate });
            });

            // PaymentDetail Configuration
            modelBuilder.Entity<PaymentDetail>(entity =>
            {
                entity.HasKey(pd => pd.Id);
                entity.Property(pd => pd.PaymentId).IsRequired();
                entity.Property(pd => pd.Method).IsRequired(); // Enum int
                entity.Property(pd => pd.Amount).HasPrecision(10, 2).IsRequired();
                entity.Property(pd => pd.TransactionCode).HasMaxLength(255);
                entity.Property(pd => pd.Provider).HasMaxLength(100);
                entity.Property(pd => pd.ExtraInfo).HasMaxLength(500);

                // Business constraints
                entity.ToTable(tb =>
                    tb.HasCheckConstraint("CK_PaymentDetail_Amount", "\"Amount\" > 0")
                );

                // Indexes for performance
                entity.HasIndex(pd => pd.PaymentId);
                entity.HasIndex(pd => pd.Method);
                entity.HasIndex(pd => pd.TransactionCode);
                entity.HasIndex(pd => pd.Provider);
            });

            // 1-1 relationship User <-> StaffProfile
            modelBuilder.Entity<User>()
                .HasOne(u => u.StaffProfile)
                .WithOne(s => s.User)
                .HasForeignKey<StaffProfile>(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

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

            modelBuilder.Entity<User>()
                .HasMany(u => u.Feedbacks)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId)
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

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.Order)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MenuItem>()
                .HasMany(m => m.OrderDetails)
                .WithOne(od => od.MenuItem)
                .HasForeignKey(od => od.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);
            // Optional relationships for 
            modelBuilder.Entity<MenuItemImage>()
                .HasOne(mi => mi.MenuItem)
                .WithMany(m => m.Images)
                .HasForeignKey(mi => mi.MenuItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasMany(p => p.PaymentDetails)
                .WithOne(pd => pd.Payment)
                .HasForeignKey(pd => pd.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional relationships for Feedback
            modelBuilder.Entity<Order>()
                .HasMany<Feedback>()
                .WithOne(f => f.Order)
                .HasForeignKey(f => f.OrderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MenuItem>()
                .HasMany<Feedback>()
                .WithOne(f => f.MenuItem)
                .HasForeignKey(f => f.MenuItemId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
