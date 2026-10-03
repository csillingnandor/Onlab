using IOMS.DTO;

namespace IOMS.DAL.Repositories;

public interface ICustomerOrderRepository
{
    Task<IReadOnlyList<CustomerOrderData>> GetAllAsync(CancellationToken ct = default);
    Task<CustomerOrderData?> GetByIdAsync(int id, CancellationToken ct = default);

    // Új, Pending státuszú rendelés a termékek aktuális árával; az azonos termékű sorokat összevonja.
    // A vevő és a termékek létezését a hívónak kell előtte ellenőriznie.
    Task<CustomerOrderData> CreateAsync(CreateCustomerOrderData data, CancellationToken ct = default);

    // Egy termék eladásai a [from, to] zárt intervallumban, napi bontással (az eladás nélküli napok 0-val).
    // Csak a kiszállított (Delivered) rendelések számítanak eladásnak.
    // null, ha nincs ilyen termék.
    Task<ProductSaleStatisticsData?> GetProductSaleStatisticsAsync(
        int productId, DateOnly from, DateOnly to, CancellationToken ct = default);
}
