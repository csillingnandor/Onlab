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
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly IValidator<CreateCustomerData> _createValidator;

    public CustomersController(ICustomerService customerService, IValidator<CreateCustomerData> createValidator)
    {
        _customerService = customerService;
        _createValidator = createValidator;
    }

    [HttpGet]
    [RequirePage(AppPages.Customers, AppPages.CustomerOrders)]
    public async Task<ActionResult<IEnumerable<CustomerData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _customerService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    [RequirePage(AppPages.Customers, AppPages.CustomerOrders)]
    public async Task<ActionResult<CustomerData>> GetById(int id, CancellationToken ct)
    {
        var customer = await _customerService.GetByIdAsync(id, ct);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    [RequireAction(AppActions.CustomersCreate)]
    public async Task<ActionResult<CustomerData>> Create(CreateCustomerData data, CancellationToken ct)
    {
        // Formai szabályok; az adatbázist igénylő ellenőrzések (létezés, egyediség) a service-ben vannak.
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _customerService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // A módosítás ugyanazokat a mezőket és szabályokat használja, mint a felvétel
    [HttpPut("{id:int}")]
    [RequireAction(AppActions.CustomersEdit)]
    public async Task<ActionResult<CustomerData>> Modify(int id, CreateCustomerData data, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _customerService.ModifyAsync(id, data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    // 409: a vevőnek vannak rendelései
    [HttpDelete("{id:int}")]
    [RequireAction(AppActions.CustomersDelete)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        return await _customerService.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
