using Microsoft.EntityFrameworkCore;
using OdiNow.Models;

namespace OdiNow.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PhoneVerification> PhoneVerifications => Set<PhoneVerification>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<RestaurantAttribute> RestaurantAttributes => Set<RestaurantAttribute>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Offer> Offers => Set<Offer>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
    public DbSet<CouponView> CouponViews => Set<CouponView>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<UserSetting> UserSettings => Set<UserSetting>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<UserTicket> UserTickets => Set<UserTicket>();
    public DbSet<CancellationRight> CancellationRights => Set<CancellationRight>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasFilter("[Email] IS NOT NULL");

        modelBuilder.Entity<User>()
            .HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Setting)
            .WithOne(s => s.User)
            .HasForeignKey<UserSetting>(s => s.UserId);

        modelBuilder.Entity<UserSetting>()
            .HasOne(s => s.DefaultAddress)
            .WithMany()
            .HasForeignKey(s => s.DefaultAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Favorite>()
            .HasIndex(f => new { f.UserId, f.FavoriteType, f.TargetId })
            .IsUnique();

        modelBuilder.Entity<SearchHistory>()
            .HasIndex(s => new { s.UserId, s.Query });

        modelBuilder.Entity<CouponView>()
            .HasIndex(c => new { c.UserId, c.OfferId });

        modelBuilder.Entity<UserTicket>()
            .HasIndex(t => new { t.UserId, t.Status });

        modelBuilder.Entity<UserTicket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserTicket>()
            .HasOne(t => t.Offer)
            .WithMany()
            .HasForeignKey(t => t.OfferId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CancellationRight>()
            .HasIndex(c => new { c.UserId, c.IsUsed });

        modelBuilder.Entity<CancellationRight>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CancellationRight>()
            .HasOne(c => c.Order)
            .WithMany()
            .HasForeignKey(c => c.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.OrderNumber)
            .IsUnique();

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany(mi => mi.OrderItems)
            .HasForeignKey(oi => oi.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        ConfigureDecimalPrecision(modelBuilder);
    }

    private static void ConfigureDecimalPrecision(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>()
            .Property(mi => mi.BasePrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Offer>()
            .Property(o => o.DiscountPercent)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.DepositAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .Property(o => o.RemainingAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.DiscountAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Restaurant>()
            .Property(r => r.AverageRating)
            .HasPrecision(3, 2);
    }
}

