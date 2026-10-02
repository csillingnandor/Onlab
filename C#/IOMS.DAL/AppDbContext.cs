using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerOrder> CustomerOrders { get; set; }
    public DbSet<CustomerOrderItem> CustomerOrderItems { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<SupplierOrder> SupplierOrders { get; set; }
    public DbSet<SupplierOrderItem> SupplierOrderItems { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CustomerOrder>(order =>
        {
            order.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

            order.HasOne(o => o.Customer)
                 .WithMany(c => c.Orders)
                 .HasForeignKey(o => o.CustomerId)
                 .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CustomerOrderItem>(item =>
        {
            item.HasOne(i => i.CustomerOrder)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.CustomerOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            item.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupplierOrder>(order =>
        {
            order.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

            order.HasOne(o => o.Supplier)
                 .WithMany(s => s.Orders)
                 .HasForeignKey(o => o.SupplierId)
                 .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupplierOrderItem>(item =>
        {
            item.HasOne(i => i.SupplierOrder)
                .WithMany(o => o.Items)
                .HasForeignKey(i => i.SupplierOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            item.HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        SeedData.Seed(modelBuilder);
    }
}
