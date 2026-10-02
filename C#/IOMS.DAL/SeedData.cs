using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOMS.DAL;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Logitech MX Master 3S egér", SKU = "PER-MX3S", Category = "Perifériák", StockQuantity = 42, MinStockLevel = 10, Price = 39990m },
            new Product { Id = 2, Name = "Keychron K2 billentyűzet", SKU = "PER-K2", Category = "Perifériák", StockQuantity = 6, MinStockLevel = 8, Price = 34990m },
            new Product { Id = 3, Name = "Dell P2723D 27\" monitor", SKU = "PER-P2723D", Category = "Perifériák", StockQuantity = 0, MinStockLevel = 5, Price = 104990m },
            new Product { Id = 4, Name = "Samsung 990 Pro 1TB SSD", SKU = "STO-990P-1T", Category = "Tárolóeszközök", StockQuantity = 25, MinStockLevel = 10, Price = 49990m },
            new Product { Id = 5, Name = "WD Red Plus 4TB HDD", SKU = "STO-WDRP-4T", Category = "Tárolóeszközök", StockQuantity = 12, MinStockLevel = 6, Price = 44990m },
            new Product { Id = 6, Name = "SanDisk Ultra 128GB pendrive", SKU = "STO-SDU-128", Category = "Tárolóeszközök", StockQuantity = 3, MinStockLevel = 15, Price = 5990m },
            new Product { Id = 7, Name = "TP-Link Archer AX55 router", SKU = "NET-AX55", Category = "Hálózati eszközök", StockQuantity = 18, MinStockLevel = 5, Price = 32990m },
            new Product { Id = 8, Name = "Ubiquiti UniFi Switch Lite 8", SKU = "NET-USL8", Category = "Hálózati eszközök", StockQuantity = 9, MinStockLevel = 4, Price = 42990m }
        );

        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Kovács Anna", Email = "anna.kovacs@example.com", Phone = "+36 30 111 2222" },
            new Customer { Id = 2, Name = "Nagy Péter", Email = "peter.nagy@example.com", Phone = "+36 20 333 4444" },
            new Customer { Id = 3, Name = "Szabó és Társa Kft.", Email = "beszerzes@szabo-kft.example.com" }
        );

        modelBuilder.Entity<Supplier>().HasData(
            new Supplier { Id = 1, Name = "TechDistri Kft.", Email = "rendeles@techdistri.example.com", Phone = "+36 1 555 1000" },
            new Supplier { Id = 2, Name = "NetParts Zrt.", Email = "sales@netparts.example.com" }
        );

        modelBuilder.Entity<Warehouse>().HasData(
            new Warehouse { Id = 1, Name = "Budapesti központi raktár", Address = "1117 Budapest, Példa utca 1.", Capacity = 5000, Latitude = 47.4730, Longitude = 19.0530 },
            new Warehouse { Id = 2, Name = "Debreceni raktár", Address = "4031 Debrecen, Minta út 12.", Capacity = 2000, Latitude = 47.5316, Longitude = 21.6273 }
        );

        modelBuilder.Entity<CustomerOrder>().HasData(
            new CustomerOrder { Id = 1, CustomerId = 1, OrderDate = new DateTime(2026, 9, 1, 9, 30, 0, DateTimeKind.Utc), Status = OrderStatus.Delivered },
            new CustomerOrder { Id = 2, CustomerId = 2, OrderDate = new DateTime(2026, 9, 10, 14, 5, 0, DateTimeKind.Utc), Status = OrderStatus.Shipped },
            new CustomerOrder { Id = 3, CustomerId = 3, OrderDate = new DateTime(2026, 9, 20, 11, 0, 0, DateTimeKind.Utc), Status = OrderStatus.Pending },
            new CustomerOrder { Id = 4, CustomerId = 1, OrderDate = new DateTime(2026, 9, 22, 16, 45, 0, DateTimeKind.Utc), Status = OrderStatus.Cancelled }
        );

        modelBuilder.Entity<CustomerOrderItem>().HasData(
            new CustomerOrderItem { Id = 1, CustomerOrderId = 1, ProductId = 1, Quantity = 2, UnitPrice = 39990m },
            new CustomerOrderItem { Id = 2, CustomerOrderId = 1, ProductId = 4, Quantity = 1, UnitPrice = 49990m },
            new CustomerOrderItem { Id = 3, CustomerOrderId = 2, ProductId = 7, Quantity = 1, UnitPrice = 32990m },
            new CustomerOrderItem { Id = 4, CustomerOrderId = 2, ProductId = 6, Quantity = 5, UnitPrice = 5990m },
            new CustomerOrderItem { Id = 5, CustomerOrderId = 3, ProductId = 3, Quantity = 3, UnitPrice = 104990m },
            new CustomerOrderItem { Id = 6, CustomerOrderId = 3, ProductId = 2, Quantity = 3, UnitPrice = 34990m },
            new CustomerOrderItem { Id = 7, CustomerOrderId = 3, ProductId = 8, Quantity = 2, UnitPrice = 42990m },
            new CustomerOrderItem { Id = 8, CustomerOrderId = 4, ProductId = 5, Quantity = 2, UnitPrice = 44990m }
        );

        modelBuilder.Entity<SupplierOrder>().HasData(
            new SupplierOrder { Id = 1, SupplierId = 1, OrderDate = new DateTime(2026, 9, 5, 8, 0, 0, DateTimeKind.Utc), Status = OrderStatus.Delivered },
            new SupplierOrder { Id = 2, SupplierId = 2, OrderDate = new DateTime(2026, 9, 21, 10, 15, 0, DateTimeKind.Utc), Status = OrderStatus.Pending }
        );

        modelBuilder.Entity<SupplierOrderItem>().HasData(
            new SupplierOrderItem { Id = 1, SupplierOrderId = 1, ProductId = 1, Quantity = 20, UnitCost = 28000m },
            new SupplierOrderItem { Id = 2, SupplierOrderId = 1, ProductId = 4, Quantity = 10, UnitCost = 36000m },
            new SupplierOrderItem { Id = 3, SupplierOrderId = 2, ProductId = 8, Quantity = 5, UnitCost = 31000m },
            new SupplierOrderItem { Id = 4, SupplierOrderId = 2, ProductId = 7, Quantity = 10, UnitCost = 23500m }
        );
    }
}
