using IOMS.BLL.Mapping;
using IOMS.DAL.Repositories;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public class CustomerOrderItemService : ICustomerOrderItemService
{
    private readonly ICustomerOrderItemRepository _items;

    public CustomerOrderItemService(ICustomerOrderItemRepository items)
    {
        _items = items;
    }

    public async Task<IReadOnlyList<CustomerOrderItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _items.GetAllAsync(ct);
        return items.Select(i => i.ToDto()).ToList();
    }

    public async Task<IReadOnlyList<CustomerOrderItemDto>> GetByOrderAsync(int orderId, CancellationToken ct = default)
    {
        var items = await _items.GetByOrderAsync(orderId, ct);
        return items.Select(i => i.ToDto()).ToList();
    }
}
