using System.Linq.Expressions;
using IOMS.DAL.Entities;
using IOMS.DTO;
using DataOrderStatus = IOMS.DTO.OrderStatus;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.DAL.Mapping;

// Entitás -> Data projekciók. Az EF SQL-re fordítja őket, így csak a szükséges oszlopok jönnek le.
internal static class DataProjections
{
    public static Expression<Func<Product, ProductData>> Product(DateTime since) => p => new ProductData
    {
        Id = p.Id,
        Name = p.Name,
        SKU = p.SKU,
        Category = p.Category,
        StockQuantity = p.StockQuantity,
        MinStockLevel = p.MinStockLevel,
        Price = p.Price,

        // Kereslet az utolsó időszakban: minden nem lemondott rendelés tételei a 'since' időponttól
        OrderedLast7Days = p.OrderItems
            .Where(i => i.CustomerOrder.Status != EntityOrderStatus.Cancelled
                     && i.CustomerOrder.OrderDate >= since)
            .Sum(i => i.Quantity),

        // Status és DaysOfCover: a repository számolja ki a lekérdezés után
    };

    public static readonly Expression<Func<CustomerOrderItem, CustomerOrderItemData>> CustomerOrderItem = i => new CustomerOrderItemData
    {
        Id = i.Id,
        CustomerOrderId = i.CustomerOrderId,
        ProductId = i.ProductId,
        ProductName = i.Product.Name,
        ProductPrice = i.UnitPrice,
        Quantity = i.Quantity,
    };

    public static readonly Expression<Func<CustomerOrder, CustomerOrderData>> CustomerOrder = o => new CustomerOrderData
    {
        Id = o.Id,
        CustomerId = o.CustomerId,
        CustomerName = o.Customer.Name,
        OrderDate = o.OrderDate,
        Status = ToData(o.Status),
        TotalAmount = o.Items.Sum(i => i.UnitPrice * i.Quantity),
        Items = o.Items
            .AsQueryable()
            .OrderBy(i => i.Id)
            .Select(CustomerOrderItem)
            .ToList(),
    };

    public static readonly Expression<Func<Warehouse, WarehouseData>> Warehouse = w => new WarehouseData
    {
        Id = w.Id,
        Name = w.Name,
        Address = w.Address,
        Capacity = w.Capacity,
        Latitude = w.Latitude,
        Longitude = w.Longitude,
    };

    public static readonly Expression<Func<SupplierOrderItem, SupplierOrderItemData>> SupplierOrderItem = i => new SupplierOrderItemData
    {
        Id = i.Id,
        SupplierOrderId = i.SupplierOrderId,
        ProductId = i.ProductId,
        ProductName = i.Product.Name,
        UnitCost = i.UnitCost,
        Quantity = i.Quantity,
    };

    public static readonly Expression<Func<SupplierOrder, SupplierOrderData>> SupplierOrder = o => new SupplierOrderData
    {
        Id = o.Id,
        SupplierId = o.SupplierId,
        SupplierName = o.Supplier.Name,
        OrderDate = o.OrderDate,
        Status = ToData(o.Status),
        TotalCost = o.Items.Sum(i => i.UnitCost * i.Quantity),
        Items = o.Items
            .AsQueryable()
            .OrderBy(i => i.Id)
            .Select(SupplierOrderItem)
            .ToList(),
    };

    public static readonly Expression<Func<Supplier, SupplierData>> Supplier = s => new SupplierData
    {
        Id = s.Id,
        Name = s.Name,
        Email = s.Email,
        Phone = s.Phone,
    };

    public static readonly Expression<Func<Customer, CustomerData>> Customer = c => new CustomerData
    {
        Id = c.Id,
        Name = c.Name,
        Email = c.Email,
        Phone = c.Phone,
        OrderCount = c.Orders.Count(),
        TotalSpent = c.Orders
            .Where(o => o.Status == EntityOrderStatus.Delivered)
            .SelectMany(o => o.Items)
            .Sum(i => i.UnitPrice * i.Quantity),
        LastOrderDate = c.Orders.Max(o => (DateTime?)o.OrderDate),
    };

    // Metódushívás, nem sima cast: így az EF a beolvasott értéken memóriában futtatja,
    // a cast-ot viszont SQL CAST-ra fordítaná, ami a stringként tárolt státusznál elszáll.
    private static DataOrderStatus ToData(EntityOrderStatus status) => status switch
    {
        EntityOrderStatus.Pending => DataOrderStatus.Pending,
        EntityOrderStatus.Shipped => DataOrderStatus.Shipped,
        EntityOrderStatus.Delivered => DataOrderStatus.Delivered,
        EntityOrderStatus.Cancelled => DataOrderStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };
}
