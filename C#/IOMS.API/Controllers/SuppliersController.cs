using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;

    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }

    // GET api/suppliers (a beszerzési rendelés űrlap választólistájához)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _supplierService.GetAllAsync(ct));
    }
}
