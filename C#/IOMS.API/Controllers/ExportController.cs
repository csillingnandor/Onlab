using IOMS.API.Infrastructure;
using IOMS.BLL.Auth;
using IOMS.BLL.Excel;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

// GET api/export/{lista}: Excel (.xlsx) letöltés; mindegyikhez az adott oldal megtekintési joga kell
[ApiController]
[Route("api/[controller]")]
public class ExportController : ControllerBase
{
    private const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpGet("products")]
    [RequirePage(AppPages.Products)]
    public async Task<IActionResult> Products(CancellationToken ct) => Xlsx(await _exportService.ProductsAsync(ct), "termekek");

    [HttpGet("customers")]
    [RequirePage(AppPages.Customers)]
    public async Task<IActionResult> Customers(CancellationToken ct) => Xlsx(await _exportService.CustomersAsync(ct), "vevok");

    [HttpGet("suppliers")]
    [RequirePage(AppPages.Suppliers)]
    public async Task<IActionResult> Suppliers(CancellationToken ct) => Xlsx(await _exportService.SuppliersAsync(ct), "beszallitok");

    [HttpGet("warehouses")]
    [RequirePage(AppPages.Warehouses)]
    public async Task<IActionResult> Warehouses(CancellationToken ct) => Xlsx(await _exportService.WarehousesAsync(ct), "raktarak");

    [HttpGet("customer-orders")]
    [RequirePage(AppPages.CustomerOrders)]
    public async Task<IActionResult> CustomerOrders(CancellationToken ct) => Xlsx(await _exportService.CustomerOrdersAsync(ct), "vevoi-rendelesek");

    [HttpGet("supplier-orders")]
    [RequirePage(AppPages.SupplierOrders)]
    public async Task<IActionResult> SupplierOrders(CancellationToken ct) => Xlsx(await _exportService.SupplierOrdersAsync(ct), "beszerzesek");

    // A fájlnévben a mai dátum, hogy több letöltés ne írja felül egymást
    private FileContentResult Xlsx(byte[] content, string name) =>
        File(content, XlsxContentType, $"{name}-{DateTime.Now:yyyy-MM-dd}.xlsx");
}
