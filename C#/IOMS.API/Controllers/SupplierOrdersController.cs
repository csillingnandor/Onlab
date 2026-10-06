using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierOrdersController : ControllerBase
{
    private readonly ISupplierOrderService _supplierOrderService;

    public SupplierOrdersController(ISupplierOrderService supplierOrderService)
    {
        _supplierOrderService = supplierOrderService;
    }

    // GET api/supplierorders
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
    public async Task<ActionResult<SupplierOrderData>> Create(CreateSupplierOrderData data, CancellationToken ct)
    {
        var created = await _supplierOrderService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
