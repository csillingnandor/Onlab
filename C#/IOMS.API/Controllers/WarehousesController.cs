using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    // GET api/warehouses
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
