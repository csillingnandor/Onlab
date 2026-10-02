using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerOrdersController : ControllerBase
{
    private readonly ICustomerOrderService _orderService;

    public CustomerOrdersController(ICustomerOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET api/customerorders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerOrderDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _orderService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerOrderDto>> GetById(int id, CancellationToken ct)
    {
        var order = await _orderService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerOrderDto>> Create(CreateCustomerOrderDto dto, CancellationToken ct)
    {
        var created = await _orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
