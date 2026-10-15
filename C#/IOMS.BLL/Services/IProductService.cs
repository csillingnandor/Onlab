using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductData>> GetAllAsync(CancellationToken ct = default);
    Task<ProductData?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ProductData> CreateAsync(CreateProductData data, CancellationToken ct = default);

    // Alapértelmezett időszak: az utolsó 30 nap (a mai nappal együtt). null, ha nincs ilyen termék.
    Task<ProductSaleStatisticsData?> GetSaleStatisticsAsync(
        int id, DateOnly? from, DateOnly? to, CancellationToken ct = default);

    // A mentett termék a számított mezőkkel együtt; null, ha nincs ilyen termék.
    Task<ProductData?> ModifyProductAsync(ModifyProductData data, CancellationToken ct = default);

    // false, ha nincs ilyen termék; ConflictException, ha rendelés hivatkozik rá.
    Task<bool> DeleteProductAsync(int id, CancellationToken ct = default);

    // A termék készlettörténete minden raktárból, legújabb elöl; null, ha nincs ilyen termék.
    Task<IReadOnlyList<StockMovementData>?> GetMovementsAsync(int id, CancellationToken ct = default);

    // Kézi korrekció: a raktári készlet a megszámolt mennyiségre áll, a különbség Adjustment mozgásként kerül a naplóba.
    // null, ha nincs ilyen termék; BusinessValidationException, ha nincs ilyen raktár vagy nincs eltérés.
    Task<ProductData?> AdjustStockAsync(int id, StockAdjustmentData data, CancellationToken ct = default);
}
