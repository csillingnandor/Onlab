using ClosedXML.Excel;
using IOMS.BLL.Services;
using IOMS.DTO;

namespace IOMS.BLL.Excel;

// Excel (.xlsx) export a listaoldalak adataiból. Ugyanazokat a service-eket használja, mint a felület,
// így a számított mezők (készlet, státusz, összegek) is ugyanúgy jelennek meg.
public interface IExportService
{
    Task<byte[]> ProductsAsync(CancellationToken ct = default);
    Task<byte[]> CustomersAsync(CancellationToken ct = default);
    Task<byte[]> SuppliersAsync(CancellationToken ct = default);
    Task<byte[]> WarehousesAsync(CancellationToken ct = default);

    // Két munkalap: a rendelések és a tételeik
    Task<byte[]> CustomerOrdersAsync(CancellationToken ct = default);
    Task<byte[]> SupplierOrdersAsync(CancellationToken ct = default);
}

internal class ExportService : IExportService
{
    private const string MoneyFormat = "#,##0 \"Ft\"";
    private const string DateFormat = "yyyy.mm.dd hh:mm";

    private readonly IProductService _products;
    private readonly ICustomerService _customers;
    private readonly ISupplierService _suppliers;
    private readonly IWarehouseService _warehouses;
    private readonly ICustomerOrderService _customerOrders;
    private readonly ISupplierOrderService _supplierOrders;

    public ExportService(IProductService products, ICustomerService customers, ISupplierService suppliers,
        IWarehouseService warehouses, ICustomerOrderService customerOrders, ISupplierOrderService supplierOrders)
    {
        _products = products;
        _customers = customers;
        _suppliers = suppliers;
        _warehouses = warehouses;
        _customerOrders = customerOrders;
        _supplierOrders = supplierOrders;
    }

    public async Task<byte[]> ProductsAsync(CancellationToken ct = default)
    {
        var rows = await _products.GetAllAsync(ct);
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Termékek", [
            new ExcelColumn<ProductData>("Azonosító", p => p.Id),
            new("Név", p => p.Name),
            new("SKU", p => p.SKU),
            new("Kategória", p => p.Category),
            new("Ár", p => p.Price, MoneyFormat),
            new("Készlet (db)", p => p.StockQuantity),
            new("Minimális készlet (db)", p => p.MinStockLevel),
            new("Állapot", p => p.Status),
            new("Rendelve (7 nap)", p => p.OrderedLast7Days),
            new("Készlet elég (nap)", p => p.DaysOfCover),
        ], rows);
        return ExcelWriter.ToBytes(workbook);
    }

    public async Task<byte[]> CustomersAsync(CancellationToken ct = default)
    {
        var rows = await _customers.GetAllAsync(ct);
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Vevők", [
            new ExcelColumn<CustomerData>("Azonosító", c => c.Id),
            new("Név", c => c.Name),
            new("E-mail", c => c.Email),
            new("Telefon", c => c.Phone),
            new("Rendelések", c => c.OrderCount),
            new("Vásárlás összesen", c => c.TotalSpent, MoneyFormat),
            new("Utolsó rendelés", c => c.LastOrderDate, DateFormat),
        ], rows);
        return ExcelWriter.ToBytes(workbook);
    }

    public async Task<byte[]> SuppliersAsync(CancellationToken ct = default)
    {
        var rows = await _suppliers.GetAllAsync(ct);
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Beszállítók", [
            new ExcelColumn<SupplierData>("Azonosító", s => s.Id),
            new("Név", s => s.Name),
            new("E-mail", s => s.Email),
            new("Telefon", s => s.Phone),
            new("Rendelések", s => s.OrderCount),
        ], rows);
        return ExcelWriter.ToBytes(workbook);
    }

    public async Task<byte[]> WarehousesAsync(CancellationToken ct = default)
    {
        var rows = await _warehouses.GetAllAsync(ct);
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Raktárak", [
            new ExcelColumn<WarehouseData>("Azonosító", w => w.Id),
            new("Név", w => w.Name),
            new("Cím", w => w.Address),
            new("Kapacitás", w => w.Capacity),
            new("Szélesség", w => w.Latitude),
            new("Hosszúság", w => w.Longitude),
        ], rows);
        return ExcelWriter.ToBytes(workbook);
    }

    public async Task<byte[]> CustomerOrdersAsync(CancellationToken ct = default)
    {
        var orders = await _customerOrders.GetAllAsync(ct);
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Rendelések", [
            new ExcelColumn<CustomerOrderData>("Rendelés", o => o.Id),
            new("Dátum", o => o.OrderDate, DateFormat),
            new("Vevő", o => o.CustomerName),
            new("Raktár", o => o.WarehouseName),
            new("Állapot", o => StatusText(o.Status)),
            new("Tételek", o => o.Items.Count),
            new("Végösszeg", o => o.TotalAmount, MoneyFormat),
        ], orders);
        ExcelWriter.AddSheet(workbook, "Tételek", [
            new ExcelColumn<(CustomerOrderData Order, CustomerOrderItemData Item)>("Rendelés", x => x.Order.Id),
            new("Vevő", x => x.Order.CustomerName),
            new("Termék", x => x.Item.ProductName),
            new("Mennyiség", x => x.Item.Quantity),
            new("Egységár", x => x.Item.ProductPrice, MoneyFormat),
            new("Összesen", x => x.Item.ProductPrice * x.Item.Quantity, MoneyFormat),
        ], orders.SelectMany(o => o.Items.Select(i => (Order: o, Item: i))));
        return ExcelWriter.ToBytes(workbook);
    }

    public async Task<byte[]> SupplierOrdersAsync(CancellationToken ct = default)
    {
        var orders = await _supplierOrders.GetAllAsync(ct);
        using var workbook = new XLWorkbook();
        ExcelWriter.AddSheet(workbook, "Beszerzések", [
            new ExcelColumn<SupplierOrderData>("Rendelés", o => o.Id),
            new("Dátum", o => o.OrderDate, DateFormat),
            new("Beszállító", o => o.SupplierName),
            new("Célraktár", o => o.WarehouseName),
            new("Állapot", o => StatusText(o.Status)),
            new("Tételek", o => o.Items.Count),
            new("Összérték", o => o.TotalCost, MoneyFormat),
        ], orders);
        ExcelWriter.AddSheet(workbook, "Tételek", [
            new ExcelColumn<(SupplierOrderData Order, SupplierOrderItemData Item)>("Rendelés", x => x.Order.Id),
            new("Beszállító", x => x.Order.SupplierName),
            new("Termék", x => x.Item.ProductName),
            new("Mennyiség", x => x.Item.Quantity),
            new("Egységár", x => x.Item.UnitCost, MoneyFormat),
            new("Összesen", x => x.Item.UnitCost * x.Item.Quantity, MoneyFormat),
        ], orders.SelectMany(o => o.Items.Select(i => (Order: o, Item: i))));
        return ExcelWriter.ToBytes(workbook);
    }

    private static string StatusText(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Függőben",
        OrderStatus.Shipped => "Szállítás alatt",
        OrderStatus.Delivered => "Kézbesítve",
        OrderStatus.Cancelled => "Lemondva",
        _ => status.ToString(),
    };
}
