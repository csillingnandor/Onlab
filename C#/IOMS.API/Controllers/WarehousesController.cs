using FluentValidation;
using IOMS.API.Infrastructure;
using IOMS.API.Validation;
using IOMS.BLL.Auth;
using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly IValidator<SaveWarehouseData> _saveValidator;

    public WarehousesController(IWarehouseService warehouseService, IValidator<SaveWarehouseData> saveValidator)
    {
        _warehouseService = warehouseService;
        _saveValidator = saveValidator;
    }

    // A rendelés űrlapok és a készletkorrekció raktárválasztójához is kell
    [HttpGet]
    [RequirePage(AppPages.Warehouses, AppPages.CustomerOrders, AppPages.SupplierOrders, AppPages.Products)]
    public async Task<ActionResult<IEnumerable<WarehouseData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _warehouseService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    [RequirePage(AppPages.Warehouses, AppPages.CustomerOrders, AppPages.SupplierOrders, AppPages.Products)]
    public async Task<ActionResult<WarehouseData>> GetById(int id, CancellationToken ct)
    {
        var warehouse = await _warehouseService.GetByIdAsync(id, ct);
        return warehouse is null ? NotFound() : Ok(warehouse);
    }

    [HttpPost]
    [RequireAction(AppActions.WarehousesCreate)]
    public async Task<ActionResult<WarehouseData>> Create(SaveWarehouseData data, CancellationToken ct)
    {
        var validation = await _saveValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _warehouseService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequireAction(AppActions.WarehousesEdit)]
    public async Task<ActionResult<WarehouseData>> Modify(int id, SaveWarehouseData data, CancellationToken ct)
    {
        var validation = await _saveValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _warehouseService.ModifyAsync(id, data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    // 409: készlet, rendelés vagy készletmozgás hivatkozik rá
    [HttpDelete("{id:int}")]
    [RequireAction(AppActions.WarehousesDelete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        return await _warehouseService.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
