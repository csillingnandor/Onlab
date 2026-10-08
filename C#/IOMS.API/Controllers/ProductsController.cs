using FluentValidation;
using IOMS.API.Validation;
using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductData> _createValidator;
    private readonly IValidator<ModifyProductData> _modifyValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductData> createValidator,
        IValidator<ModifyProductData> modifyValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _modifyValidator = modifyValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _productService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductData>> GetById(int id, CancellationToken ct)
    {
        var product = await _productService.GetByIdAsync(id, ct);
        return product is null ? NotFound() : Ok(product);
    }

    // GET api/products/5/sales?from=2026-09-01&to=2026-09-30
    [HttpGet("{id:int}/sales")]
    public async Task<ActionResult<ProductSaleStatisticsData>> GetSaleStatistics(
        int id, [FromQuery] DateOnly? from, [FromQuery] DateOnly? to, CancellationToken ct)
    {
        var statistics = await _productService.GetSaleStatisticsAsync(id, from, to, ct);
        return statistics is null ? NotFound() : Ok(statistics);
    }

    [HttpPost]
    public async Task<ActionResult<ProductData>> Create(CreateProductData data, CancellationToken ct)
    {
        // Formai szabályok; az adatbázist igénylő ellenőrzések (létezés, egyediség) a service-ben vannak.
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _productService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProductData>> Modify(int id, ModifyProductData data, CancellationToken ct)
    {
        // Az útvonalbeli azonosító a mérvadó, a törzsben küldött Id-t felülírjuk.
        data.Id = id;

        var validation = await _modifyValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _productService.ModifyProductAsync(data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        return await _productService.DeleteProductAsync(id, ct) ? NoContent() : NotFound();
    }
}
