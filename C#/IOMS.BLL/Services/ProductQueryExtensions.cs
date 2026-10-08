using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace IOMS.BLL.Services;

internal static class ProductQueryExtensions
{
    // A megadott azonosítók közül azok, amelyekhez nincs termék.
    public static async Task<IReadOnlyList<int>> GetMissingIdsAsync(
        this IQueryable<Product> products, IEnumerable<int> productIds, CancellationToken ct = default)
    {
        var ids = productIds.Distinct().ToList();
        var existing = await products
            .Where(p => ids.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);

        return ids.Except(existing).ToList();
    }
}
