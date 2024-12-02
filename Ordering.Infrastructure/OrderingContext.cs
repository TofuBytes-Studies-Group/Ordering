using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure;

public class OrderingContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");
            entity.HasKey(o => o.Id);
            entity.Property(o => o.CustomerId).IsRequired();
            entity.Property(o => o.RestaurantId).IsRequired();
            entity.Property(o => o.TotalPrice).IsRequired();
            entity.HasMany(o => o.OrderLines).WithOne(ol => ol.Order).HasForeignKey(ol => ol.OrderId);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.ToTable("Orderline");
            entity.HasKey(ol => ol.Id);
            entity.Property(ol => ol.Quantity).IsRequired();
            entity.Property(ol => ol.Price).IsRequired();
            entity.Property(ol => ol.OrderId).HasColumnName("OrderId");
            entity.HasOne(ol => ol.Order).WithMany(o => o.OrderLines).HasForeignKey(ol => ol.OrderId);
        });
    }
}
