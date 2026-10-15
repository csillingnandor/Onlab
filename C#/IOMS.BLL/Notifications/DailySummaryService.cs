using System.Globalization;
using IOMS.BLL.Auth;
using IOMS.DAL;
using IOMS.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IOMS.BLL.Notifications;

// Napi összefoglaló az adminisztrátoroknak: az előző nap eladásai, a függő rendelések és az alacsony készletű termékek.
public interface IDailySummaryService
{
    // A levelet sorba teszi; visszaadja, hány címzettnek megy (0, ha egy adminnak sincs e-mail címe)
    Task<int> SendAsync(DateOnly day, CancellationToken ct = default);
}

internal class DailySummaryService : IDailySummaryService
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailQueue _queue;
    private readonly EmailOptions _options;

    public DailySummaryService(AppDbContext context, UserManager<AppUser> userManager, IEmailQueue queue,
        IOptions<EmailOptions> options)
    {
        _context = context;
        _userManager = userManager;
        _queue = queue;
        _options = options.Value;
    }

    public async Task<int> SendAsync(DateOnly day, CancellationToken ct = default)
    {
        var recipients = (await _userManager.GetUsersInRoleAsync(AppRoles.Admin))
            .Select(u => u.Email)
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e!)
            .Distinct()
            .ToList();
        if (recipients.Count == 0)
            return 0;

        // A nap határai helyi idő szerint, UTC-ben tárolt időpontokhoz
        var from = day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Local).ToUniversalTime();
        var to = from.AddDays(1);

        var newOrders = await _context.CustomerOrders.CountAsync(o => o.OrderDate >= from && o.OrderDate < to, ct);
        // Az adott napon kiszállított (Shipment mozgás) és azóta kézbesített rendelések bevétele
        var shippedOrderIds = _context.StockMovements
            .Where(m => m.Type == StockMovementType.Shipment && m.CreatedAt >= from && m.CreatedAt < to && m.CustomerOrderId != null)
            .Select(m => m.CustomerOrderId!.Value)
            .Distinct();
        var shippedRevenue = await _context.CustomerOrderItems
            .Where(i => shippedOrderIds.Contains(i.CustomerOrderId))
            .SumAsync(i => (decimal?)(i.UnitPrice * i.Quantity), ct) ?? 0;
        var shippedCount = await shippedOrderIds.CountAsync(ct);

        var pendingCustomer = await _context.CustomerOrders.CountAsync(o => o.Status == OrderStatus.Pending, ct);
        var pendingSupplier = await _context.SupplierOrders.CountAsync(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Shipped, ct);

        var lowStock = await _context.Products
            .AsNoTracking()
            .Select(p => new { p.Name, p.SKU, p.MinStockLevel, Stock = p.Inventories.Sum(i => (int?)i.Quantity) ?? 0 })
            .Where(p => p.Stock <= p.MinStockLevel)
            .OrderBy(p => p.Stock)
            .ToListAsync(ct);

        var dayText = day.ToString("yyyy. MM. dd.", CultureInfo.GetCultureInfo("hu-HU"));
        var summary = EmailTemplates.Table(
            ["Mutató", "Érték"],
            [
                ["Új vevői rendelések", $"{newOrders} db"],
                ["Kiszállított rendelések", $"{shippedCount} db"],
                ["Kiszállítások értéke", EmailTemplates.Money(shippedRevenue)],
                ["Függő vevői rendelések (most)", $"{pendingCustomer} db"],
                ["Úton lévő beszerzések (most)", $"{pendingSupplier} db"],
                ["Alacsony készletű termékek (most)", $"{lowStock.Count} db"],
            ],
            1);
        var lowStockTable = lowStock.Count == 0
            ? string.Empty
            : "<h3 style=\"font-size:15px;margin:20px 0 8px\">Alacsony készlet</h3>" + EmailTemplates.Table(
                ["Termék", "SKU", "Készlet", "Minimum"],
                lowStock.Select(p => (IReadOnlyList<string>)[p.Name, p.SKU, $"{p.Stock} db", $"{p.MinStockLevel} db"]),
                2, 3);

        var body = EmailTemplates.Layout(
            $"Napi összefoglaló – {dayText}",
            [$"A(z) {dayText} nap összesítése:"],
            summary + lowStockTable,
            $"{_options.AppBaseUrl}/dashboard", "Műszerfal megnyitása");
        _queue.Enqueue(new EmailMessage(recipients, $"IOMS napi összefoglaló – {dayText}", body));
        return recipients.Count;
    }
}

// Minden nap az EmailOptions.DailySummaryTime időpontban elküldi az előző napi összefoglalót
internal class DailySummaryBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EmailOptions _options;
    private readonly ILogger<DailySummaryBackgroundService> _logger;

    public DailySummaryBackgroundService(IServiceScopeFactory scopeFactory, IOptions<EmailOptions> options,
        ILogger<DailySummaryBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!TimeOnly.TryParseExact(_options.DailySummaryTime, "HH:mm", out var sendAt))
        {
            _logger.LogInformation("Nincs (érvényes) Email:DailySummaryTime beállítás, a napi összefoglaló ki van kapcsolva.");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now;
            var next = DateOnly.FromDateTime(now).ToDateTime(sendAt);
            if (next <= now)
                next = next.AddDays(1);

            await Task.Delay(next - now, stoppingToken);

            try
            {
                await using var scope = _scopeFactory.CreateAsyncScope();
                var summary = scope.ServiceProvider.GetRequiredService<IDailySummaryService>();
                var count = await summary.SendAsync(DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), stoppingToken);
                _logger.LogInformation("Napi összefoglaló sorba téve {Count} címzettnek.", count);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Nem sikerült elkészíteni a napi összefoglalót.");
            }
        }
    }
}
