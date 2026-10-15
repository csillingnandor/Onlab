using AutoMapper;
using IOMS.DAL.Entities;
using IOMS.DTO;
using DataMovementType = IOMS.DTO.StockMovementType;
using DataOrderStatus = IOMS.DTO.OrderStatus;
using EntityMovementType = IOMS.DAL.Entities.StockMovementType;
using EntityOrderStatus = IOMS.DAL.Entities.OrderStatus;

namespace IOMS.BLL.Mapping;

// Entitás -> Data leképezések. A service-ek ProjectTo-val használják, így az EF SQL-re fordítja őket,
// és csak a szükséges oszlopok jönnek le. A navigációs nevek (pl. CustomerName <- Customer.Name)
// konvenció alapján képződnek le; a számított mezőket a MapFrom adja meg.
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // A státusz és a mozgástípus szövegként van tárolva. Sima enum-konverziót az EF SQL CAST-ra fordítana,
        // ami a szöveges oszlopon elszáll; a feltételes kifejezésből viszont SQL CASE lesz.
        CreateProjection<EntityOrderStatus, DataOrderStatus>().ConvertUsing(s =>
            s == EntityOrderStatus.Pending ? DataOrderStatus.Pending
            : s == EntityOrderStatus.Shipped ? DataOrderStatus.Shipped
            : s == EntityOrderStatus.Delivered ? DataOrderStatus.Delivered
            : DataOrderStatus.Cancelled);

        CreateProjection<EntityMovementType, DataMovementType>().ConvertUsing(t =>
            t == EntityMovementType.Opening ? DataMovementType.Opening
            : t == EntityMovementType.Receipt ? DataMovementType.Receipt
            : t == EntityMovementType.Shipment ? DataMovementType.Shipment
            : DataMovementType.Adjustment);

        // ---------- Termék
        // A 'since' időpontot a ProductService adja át a ProjectTo paramétereként (az utolsó 7 nap kezdete)
        DateTime since = default;

        CreateProjection<Product, ProductData>()
            .ForMember(d => d.StockQuantity, o => o.MapFrom(p => p.Inventories.Sum(i => i.Quantity)))
            // Kereslet az utolsó időszakban: minden nem lemondott rendelés tételei a 'since' időponttól
            .ForMember(d => d.OrderedLast7Days, o => o.MapFrom(p => p.OrderItems
                .Where(i => i.CustomerOrder.Status != EntityOrderStatus.Cancelled && i.CustomerOrder.OrderDate >= since)
                .Sum(i => i.Quantity)))
            .ForMember(d => d.Stocks, o => o.MapFrom(p => p.Inventories.OrderBy(i => i.Warehouse.Name)))
            // Status és DaysOfCover: a ProductService számolja ki a lekérdezés után
            .ForMember(d => d.Status, o => o.Ignore())
            .ForMember(d => d.DaysOfCover, o => o.Ignore());

        CreateProjection<Inventory, ProductStockData>();

        CreateProjection<StockMovement, StockMovementData>();

        // ---------- Vevő
        CreateProjection<Customer, CustomerData>()
            .ForMember(d => d.OrderCount, o => o.MapFrom(c => c.Orders.Count()))
            // Csak a kiszállított (Delivered) rendelések számítanak eladásnak
            .ForMember(d => d.TotalSpent, o => o.MapFrom(c => c.Orders
                .Where(ord => ord.Status == EntityOrderStatus.Delivered)
                .SelectMany(ord => ord.Items)
                .Sum(i => i.UnitPrice * i.Quantity)))
            .ForMember(d => d.LastOrderDate, o => o.MapFrom(c => c.Orders.Max(ord => (DateTime?)ord.OrderDate)));

        // ---------- Vevői rendelés
        CreateProjection<CustomerOrderItem, CustomerOrderItemData>()
            .ForMember(d => d.ProductPrice, o => o.MapFrom(i => i.UnitPrice));

        CreateProjection<CustomerOrder, CustomerOrderData>()
            .ForMember(d => d.TotalAmount, o => o.MapFrom(ord => ord.Items.Sum(i => i.UnitPrice * i.Quantity)))
            .ForMember(d => d.Items, o => o.MapFrom(ord => ord.Items.OrderBy(i => i.Id)));

        // ---------- Beszállító, beszerzési rendelés
        CreateProjection<Supplier, SupplierData>()
            .ForMember(d => d.OrderCount, o => o.MapFrom(s => s.Orders.Count()));

        CreateProjection<SupplierOrderItem, SupplierOrderItemData>();

        CreateProjection<SupplierOrder, SupplierOrderData>()
            .ForMember(d => d.TotalCost, o => o.MapFrom(ord => ord.Items.Sum(i => i.UnitCost * i.Quantity)))
            .ForMember(d => d.Items, o => o.MapFrom(ord => ord.Items.OrderBy(i => i.Id)));

        // ---------- Raktár
        CreateProjection<Warehouse, WarehouseData>();
    }
}
