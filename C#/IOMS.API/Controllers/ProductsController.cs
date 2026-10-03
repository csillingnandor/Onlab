using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
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
        var created = await _productService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
