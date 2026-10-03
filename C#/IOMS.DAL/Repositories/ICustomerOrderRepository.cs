using IOMS.DAL.QueryResults;
using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ICustomerOrderRepository
{
    Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default);

    // Új, Pending státuszú rendelés a termékek aktuális árával; az azonos termékű sorokat összevonja.
    // A vevő és a termékek létezését a hívónak kell előtte ellenőriznie.
    Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default);

    // Csak a kiszállított (Delivered) rendelések számítanak eladásnak; az intervallum [from, toExclusive).
    Task<IReadOnlyList<DailySalesResult>> GetDailySalesForProductAsync(
        int productId, DateTime from, DateTime toExclusive, CancellationToken ct = default);
}
