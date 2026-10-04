using System.Linq.Expressions;
using IOMS.DAL.Entities;
using IOMS.DTO;
using DataOrderStatus = IOMS.DTO.OrderStatus;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.DAL.Mapping;

// Entitás -> Data projekciók. Az EF SQL-re fordítja őket, így csak a szükséges oszlopok jönnek le.
internal static class DataProjections
{
    public static readonly Expression<Func<Product, ProductData>> Product = p => new ProductData
    {
        Id = p.Id,
        Name = p.Name,
        SKU = p.SKU,
        Category = p.Category,
        StockQuantity = p.StockQuantity,
        MinStockLevel = p.MinStockLevel,
        Price = p.Price,
        // Üzleti szabály: készlethiány / alacsony készlet a minimum szinthez képest.
        Status = p.StockQuantity == 0 ? "Out of Stock" :
                 p.StockQuantity <= p.MinStockLevel ? "Low Stock" :
                 "In Stock",
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
