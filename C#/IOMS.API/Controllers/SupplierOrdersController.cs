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
[RequirePage(AppPages.SupplierOrders)]
public class SupplierOrdersController : ControllerBase
{
    private readonly ISupplierOrderService _supplierOrderService;
    private readonly IValidator<CreateSupplierOrderData> _createValidator;

    public SupplierOrdersController(ISupplierOrderService supplierOrderService, IValidator<CreateSupplierOrderData> createValidator)
    {
        _supplierOrderService = supplierOrderService;
        _createValidator = createValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierOrderData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _supplierOrderService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierOrderData>> GetById(int id, CancellationToken ct)
    {
        var order = await _supplierOrderService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [RequireAction(AppActions.SupplierOrdersCreate)]
    public async Task<ActionResult<SupplierOrderData>> Create(CreateSupplierOrderData data, CancellationToken ct)
    {
        // Formai szabályok; az adatbázist igénylő ellenőrzések (létezés, egyediség) a service-ben vannak.
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _supplierOrderService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT api/supplierorders/5: függő beszerzés módosítása (409, ha már nem függő)
    [HttpPut("{id:int}")]
    [RequireAction(AppActions.SupplierOrdersEdit)]
    public async Task<ActionResult<SupplierOrderData>> Modify(int id, CreateSupplierOrderData data, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _supplierOrderService.ModifyAsync(id, data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    // POST api/supplierorders/5/receive: beérkeztetés a célraktárba (409, ha már beérkezett vagy le van mondva)
    [HttpPost("{id:int}/receive")]
    [RequireAction(AppActions.SupplierOrdersReceive)]
    public async Task<ActionResult<SupplierOrderData>> Receive(int id, CancellationToken ct)
    {
        var received = await _supplierOrderService.ReceiveAsync(id, ct);
        return received is null ? NotFound() : Ok(received);
    }
}
