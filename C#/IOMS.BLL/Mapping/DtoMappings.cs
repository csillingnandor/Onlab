using IOMS.DAL.Entities;
using IOMS.DTO;
using DtoOrderStatus = IOMS.DTO.OrderStatus;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Mapping;

// Entitás -> DTO átalakítás. A repository-k által betöltött entitásokon fut, memóriában.
internal static class DtoMappings
{
    public static ProductDto ToDto(this Product p) =>
        new(
            p.Id,
            p.Name,
            p.SKU,
            p.Category,
            p.StockQuantity,
            p.MinStockLevel,
            p.Price,
            GetStockStatus(p));

    // Feltétel: a Product navigációs property be van töltve.
    public static CustomerOrderItemDto ToDto(this CustomerOrderItem i) =>
        new(
            i.Id,
            i.CustomerOrderId,
            i.ProductId,
            i.Product.Name,
            i.UnitPrice,
            i.Quantity);

    // Feltétel: a Customer és az Items (Product-tal együtt) be van töltve.
    public static CustomerOrderDto ToDto(this CustomerOrder o) =>
        new(
            o.Id,
            o.CustomerId,
            o.Customer.Name,
            o.OrderDate,
            o.Status.ToDto(),
            o.Items.Sum(i => i.UnitPrice * i.Quantity),
            o.Items.Select(i => i.ToDto()).ToList());

    public static DtoOrderStatus ToDto(this EntityOrderStatus status) => status switch
    {
        EntityOrderStatus.Pending => DtoOrderStatus.Pending,
        EntityOrderStatus.Shipped => DtoOrderStatus.Shipped,
        EntityOrderStatus.Delivered => DtoOrderStatus.Delivered,
        EntityOrderStatus.Cancelled => DtoOrderStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, null),
    };

    // Üzleti szabály: készlethiány / alacsony készlet a minimum szinthez képest.
    private static string GetStockStatus(Product p) =>
        p.StockQuantity == 0 ? "Out of Stock" :
        p.StockQuantity <= p.MinStockLevel ? "Low Stock" :
        "In Stock";
}
