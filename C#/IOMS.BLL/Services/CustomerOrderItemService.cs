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

    public Task<IReadOnlyList<CustomerOrderItemData>> GetAllAsync(CancellationToken ct = default)
    {
        return _items.GetAllAsync(ct);
    }

    public Task<IReadOnlyList<CustomerOrderItemData>> GetByOrderAsync(int orderId, CancellationToken ct = default)
    {
        return _items.GetByOrderAsync(orderId, ct);
    }
}
