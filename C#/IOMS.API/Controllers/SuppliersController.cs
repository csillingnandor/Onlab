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
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    private readonly IValidator<SaveSupplierData> _saveValidator;

    public SuppliersController(ISupplierService supplierService, IValidator<SaveSupplierData> saveValidator)
    {
        _supplierService = supplierService;
        _saveValidator = saveValidator;
    }

    // GET api/suppliers (a Beszállítók oldalnak és a beszerzési rendelés űrlap választólistájához)
    [HttpGet]
    [RequirePage(AppPages.Suppliers, AppPages.SupplierOrders)]
    public async Task<ActionResult<IEnumerable<SupplierData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _supplierService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    [RequirePage(AppPages.Suppliers, AppPages.SupplierOrders)]
    public async Task<ActionResult<SupplierData>> GetById(int id, CancellationToken ct)
    {
        var supplier = await _supplierService.GetByIdAsync(id, ct);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    [RequireAction(AppActions.SuppliersCreate)]
    public async Task<ActionResult<SupplierData>> Create(SaveSupplierData data, CancellationToken ct)
    {
        var validation = await _saveValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _supplierService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [RequireAction(AppActions.SuppliersEdit)]
    public async Task<ActionResult<SupplierData>> Modify(int id, SaveSupplierData data, CancellationToken ct)
    {
        var validation = await _saveValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _supplierService.ModifyAsync(id, data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    // 409: a beszállítónak vannak rendelései
    [HttpDelete("{id:int}")]
    [RequireAction(AppActions.SuppliersDelete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        return await _supplierService.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
