using LuxuryResort.Areas.Admin.Models;
using LuxuryResort.Areas.Identity.Data;
using LuxuryResort.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LuxuryResort.Data;

public class LuxuryResortContext : IdentityDbContext<LuxuryResortUser>
{
    public LuxuryResortContext(DbContextOptions<LuxuryResortContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Configure decimal precision for monetary values
        builder.Entity<Models.Booking>()
            .Property(b => b.TotalAmount)
            .HasColumnType("decimal(18,2)");
            
        builder.Entity<Models.Room>()
            .Property(r => r.PricePerNight)
            .HasColumnType("decimal(18,2)");
            
        // Configure string length for better performance
        builder.Entity<Models.Booking>()
            .Property(b => b.Status)
            .HasMaxLength(50);
            
        builder.Entity<Models.Booking>()
            .Property(b => b.PaymentMethod)
            .HasMaxLength(50);
            
        builder.Entity<Models.Booking>()
            .Property(b => b.ConfirmationCode)
            .HasMaxLength(20);
            
        builder.Entity<Models.Booking>()
            .Property(b => b.PaymentCode)
            .HasMaxLength(10);
            
        builder.Entity<Models.Booking>()
            .Property(b => b.SpecialRequest)
            .HasMaxLength(1000);
            
        // Configure indexes for better query performance
        builder.Entity<Models.Booking>()
            .HasIndex(b => b.UserId);
            
        builder.Entity<Models.Booking>()
            .HasIndex(b => b.RoomInstanceId);
            
        builder.Entity<Models.Booking>()
            .HasIndex(b => b.Status);
            
        builder.Entity<Models.Booking>()
            .HasIndex(b => b.CheckInDate);
            
        builder.Entity<Models.Booking>()
            .HasIndex(b => b.CheckOutDate);
    }

    public DbSet<Models.Room> Rooms { get; set; } 
    public DbSet<Booking> Bookings { get; set; }

    public DbSet<RoomInstance> RoomInstances { get; set; }
}
