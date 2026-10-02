using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerOrderItemsController : ControllerBase
{
    private readonly ICustomerOrderItemService _itemService;

    public CustomerOrderItemsController(ICustomerOrderItemService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerOrderItemDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _itemService.GetAllAsync(ct));
    }

    [HttpGet("by-order/{orderId:int}")]
    public async Task<ActionResult<IEnumerable<CustomerOrderItemDto>>> GetByOrder(int orderId, CancellationToken ct)
    {
        return Ok(await _itemService.GetByOrderAsync(orderId, ct));
    }
}
