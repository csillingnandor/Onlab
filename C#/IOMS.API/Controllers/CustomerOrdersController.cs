using FluentValidation;
using IOMS.API.Validation;
using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerOrdersController : ControllerBase
{
    private readonly ICustomerOrderService _orderService;
    private readonly IValidator<CreateCustomerOrderData> _createValidator;

    public CustomerOrdersController(ICustomerOrderService orderService, IValidator<CreateCustomerOrderData> createValidator)
    {
        _orderService = orderService;
        _createValidator = createValidator;
    }

    // GET api/customerorders
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerOrderData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _orderService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerOrderData>> GetById(int id, CancellationToken ct)
    {
        var order = await _orderService.GetByIdAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerOrderData>> Create(CreateCustomerOrderData data, CancellationToken ct)
    {
        // Formai szabályok; az adatbázist igénylő ellenőrzések (létezés, egyediség) a service-ben vannak.
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _orderService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // POST api/customerorders/5/ship: kiszállítás, a kiszolgáló raktár készlete csökken (409, ha nincs elég)
    [HttpPost("{id:int}/ship")]
    public async Task<ActionResult<CustomerOrderData>> Ship(int id, CancellationToken ct)
    {
        var order = await _orderService.ShipAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    // POST api/customerorders/5/deliver: kézbesítve (csak kiszállított rendelésnél)
    [HttpPost("{id:int}/deliver")]
    public async Task<ActionResult<CustomerOrderData>> Deliver(int id, CancellationToken ct)
    {
        var order = await _orderService.DeliverAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }

    // POST api/customerorders/5/cancel: lemondás (csak függő rendelésnél)
    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<CustomerOrderData>> Cancel(int id, CancellationToken ct)
    {
        var order = await _orderService.CancelAsync(id, ct);
        return order is null ? NotFound() : Ok(order);
    }
}
