using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure;

public class OrderingContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
    public DbSet<Dish> Dishes => Set<Dish>();

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
            
            entity.Property(o => o.CustomerName).IsRequired();
            entity.Property(o => o.CustomerEmail).IsRequired();
            entity.Property(o => o.CustomerPhoneNumber).IsRequired();
            entity.Property(o => o.CustomerAddress).IsRequired();
            entity.Property(o => o.RestaurantName).IsRequired();
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.ToTable("Orderline");
            entity.HasKey(ol => ol.Id);
            entity.HasOne<Dish>().WithMany().HasForeignKey("Dish_Id");
            entity.Property(ol => ol.Quantity).IsRequired();
            entity.Property(ol => ol.Price).IsRequired();
            entity.Property(ol => ol.DishName).IsRequired(); 
            entity.HasOne<Order>().WithMany(o => o.OrderLines).HasForeignKey(ol => ol.Order_Id);
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.ToTable("Dish");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired();
            entity.Property(d => d.Price).IsRequired();
        });
    }
}
