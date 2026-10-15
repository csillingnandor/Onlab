using IOMS.API.Infrastructure;
using IOMS.BLL.Auth;
using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequirePage(AppPages.Warehouses, AppPages.CustomerOrders, AppPages.SupplierOrders)]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _warehouseService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseData>> GetById(int id, CancellationToken ct)
    {
        var warehouse = await _warehouseService.GetByIdAsync(id, ct);
        return warehouse is null ? NotFound() : Ok(warehouse);
    }
}
