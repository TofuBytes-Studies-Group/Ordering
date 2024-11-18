﻿using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Domain.Aggregates;

namespace Ordering.Infrastructure;

public class OrderingContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderLine> OrderLines { get; set; }
    public DbSet<Dish> Dishes { get; set; }

    public OrderingContext(DbContextOptions<OrderingContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);
            
            entity.Property(o => o.CustomerName).IsRequired();
            entity.Property(o => o.CustomerEmail).IsRequired();
            entity.Property(o => o.CustomerPhoneNumber).IsRequired();
            entity.Property(o => o.CustomerAddress).IsRequired();
            
            entity.Property(o => o.TotalPrice).HasComputedColumnSql("[... SQL Here ...]");
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(ol => ol.Id);
            entity.HasOne<Dish>().WithMany().HasForeignKey("DishId");
            entity.Property(ol => ol.Quantity).IsRequired();
            entity.Property(ol => ol.Price).IsRequired();
            entity.HasOne<Order>().WithMany(o => o.OrderLines).HasForeignKey(ol => ol.OrderId);
        });

        modelBuilder.Entity<Dish>(entity =>
        {
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired();
            entity.Property(d => d.Price).IsRequired();
        });
    }
}