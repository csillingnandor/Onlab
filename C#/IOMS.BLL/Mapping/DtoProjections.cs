using System.Linq.Expressions;
using IOMS.DAL.Entities;
using IOMS.DTO;
using DtoOrderStatus = IOMS.DTO.OrderStatus;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Mapping;

// EF által SQL-re fordítható projekciók: csak a szükséges oszlopok jönnek le az adatbázisból.
internal static class DtoProjections
{
    public static readonly Expression<Func<Product, ProductDto>> Product = p =>
        new ProductDto(
            p.Id,
            p.Name,
            p.SKU,
            p.Category,
            p.StockQuantity,
            p.MinStockLevel,
            p.Price,
            p.StockQuantity == 0 ? "Out of Stock" :
            p.StockQuantity <= p.MinStockLevel ? "Low Stock" :
            "In Stock");

    public static readonly Expression<Func<CustomerOrderItem, CustomerOrderItemDto>> CustomerOrderItem = i =>
        new CustomerOrderItemDto(
            i.Id,
            i.CustomerOrderId,
            i.ProductId,
            i.Product.Name,
            i.UnitPrice,
            i.Quantity);

    public static readonly Expression<Func<CustomerOrder, CustomerOrderDto>> CustomerOrder = o =>
        new CustomerOrderDto(
            o.Id,
            o.CustomerId,
            o.Customer.Name,
            o.OrderDate,
            ToDto(o.Status),
            o.Items.Sum(i => i.UnitPrice * i.Quantity),
            o.Items
                .AsQueryable()
                .OrderBy(i => i.Id)
                .Select(CustomerOrderItem)
                .ToList());

    // Metódushívás, nem sima cast: így az EF a beolvasott értéken memóriában futtatja,
    // a cast-ot viszont SQL CAST-ra fordítaná, ami a stringként tárolt státusznál elszáll.
    private static DtoOrderStatus ToDto(EntityOrderStatus status) => status switch
    {
        EntityOrderStatus.Pending => DtoOrderStatus.Pending,
        EntityOrderStatus.Shipped => DtoOrderStatus.Shipped,
        EntityOrderStatus.Delivered => DtoOrderStatus.Delivered,
        EntityOrderStatus.Cancelled => DtoOrderStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };
}
