using IOMS.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL;

// Az Identity táblák (AspNetUsers, AspNetRoles, AspNetRoleClaims...) is ebben a kontextusban vannak
public class AppDbContext : IdentityDbContext<AppUser>
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
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<Rack> Racks { get; set; }
    public DbSet<StorageLocation> StorageLocations { get; set; }
    public DbSet<LocationStock> LocationStocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Entitásonkénti leképezés és seed adatok: Configurations/*Configuration.cs
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
