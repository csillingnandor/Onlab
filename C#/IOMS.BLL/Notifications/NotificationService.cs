using IOMS.BLL.Auth;
using IOMS.DAL;
using IOMS.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IOMS.BLL.Notifications;

// Eseményértesítések e-mailben. A címzettek azok a felhasználók (megadott e-mail címmel), akiknek valamelyik
// szerepköre eléri az érintett oldalt; a rendelés státuszáról a vevő kap levelet.
// A metódusok csak sorba teszik a levelet, és soha nem dobnak: egy értesítés hibája nem buktathatja el a műveletet.
public interface INotificationService
{
    Task CustomerOrderCreatedAsync(CustomerOrderData order, CancellationToken ct = default);
    Task CustomerOrderStatusChangedAsync(CustomerOrderData order, CancellationToken ct = default);
    Task SupplierOrderReceivedAsync(SupplierOrderData order, CancellationToken ct = default);

    // Készletcsökkenés után: azokról a termékekről megy levél, amelyek most estek a minimum szint alá (vagy fogytak el)
    Task StockDecreasedAsync(IReadOnlyDictionary<int, int> decreaseByProduct, CancellationToken ct = default);
}

internal class NotificationService : INotificationService
{
    private readonly AppDbContext _context;
    private readonly IEmailQueue _queue;
    private readonly EmailOptions _options;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(AppDbContext context, IEmailQueue queue, IOptions<EmailOptions> options,
        ILogger<NotificationService> logger)
    {
        _context = context;
        _queue = queue;
        _options = options.Value;
        _logger = logger;
    }

    public Task CustomerOrderCreatedAsync(CustomerOrderData order, CancellationToken ct = default) => SafeAsync(async () =>
    {
        var recipients = await StaffWithPageAsync(AppPages.CustomerOrders, ct);
        var body = EmailTemplates.Layout(
            $"Új vevői rendelés #{order.Id}",
            [
                $"Új rendelés érkezett: <strong>{EmailTemplates.Encode(order.CustomerName)}</strong>, " +
                $"kiszolgáló raktár: {EmailTemplates.Encode(order.WarehouseName)}.",
                $"Végösszeg: <strong>{EmailTemplates.Money(order.TotalAmount)}</strong>",
            ],
            OrderItemsTable(order),
            $"{_options.AppBaseUrl}/customer-orders", "Rendelések megnyitása");
        _queue.Enqueue(new EmailMessage(recipients, $"Új vevői rendelés #{order.Id} – {order.CustomerName}", body));
    }, "új rendelés");

    public Task CustomerOrderStatusChangedAsync(CustomerOrderData order, CancellationToken ct = default) => SafeAsync(async () =>
    {
        var email = await _context.Customers
            .Where(c => c.Id == order.CustomerId)
            .Select(c => c.Email)
            .SingleOrDefaultAsync(ct);
        if (string.IsNullOrWhiteSpace(email))
            return;

        var (subject, text) = order.Status switch
        {
            OrderStatus.Shipped => ("kiszállítás alatt", "Rendelését átadtuk a futárnak, hamarosan megérkezik."),
            OrderStatus.Delivered => ("kézbesítve", "Rendelését kézbesítettük. Köszönjük a vásárlást!"),
            OrderStatus.Cancelled => ("lemondva", "Rendelését lemondtuk. Kérdés esetén keressen minket."),
            _ => (null, null),
        };
        if (subject is null)
            return;

        var body = EmailTemplates.Layout(
            $"#{order.Id} rendelés – {subject}",
            [$"Kedves {EmailTemplates.Encode(order.CustomerName)}!", text!,
             $"Rendelés dátuma: {EmailTemplates.Date(order.OrderDate)}, végösszeg: {EmailTemplates.Money(order.TotalAmount)}"],
            OrderItemsTable(order));
        _queue.Enqueue(new EmailMessage([email], $"#{order.Id} rendelése {subject}", body));
    }, "rendelés státusza");

    public Task SupplierOrderReceivedAsync(SupplierOrderData order, CancellationToken ct = default) => SafeAsync(async () =>
    {
        var recipients = await StaffWithPageAsync(AppPages.SupplierOrders, ct);
        var table = EmailTemplates.Table(
            ["Termék", "Mennyiség", "Egységár"],
            order.Items.Select(i => (IReadOnlyList<string>)[i.ProductName, $"{i.Quantity} db", EmailTemplates.Money(i.UnitCost)]),
            1, 2);
        var body = EmailTemplates.Layout(
            $"Beérkezett beszerzés #{order.Id}",
            [$"A(z) <strong>{EmailTemplates.Encode(order.SupplierName)}</strong> szállítmánya beérkezett a(z) " +
             $"{EmailTemplates.Encode(order.WarehouseName)} raktárba.",
             $"Összérték: {EmailTemplates.Money(order.TotalCost)}"],
            table,
            $"{_options.AppBaseUrl}/supplier-orders", "Beszerzések megnyitása");
        _queue.Enqueue(new EmailMessage(recipients, $"Beérkezett beszerzés #{order.Id} – {order.SupplierName}", body));
    }, "beérkezett beszerzés");

    public Task StockDecreasedAsync(IReadOnlyDictionary<int, int> decreaseByProduct, CancellationToken ct = default) => SafeAsync(async () =>
    {
        if (decreaseByProduct.Count == 0)
            return;

        var ids = decreaseByProduct.Keys.ToList();
        var products = await _context.Products
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.SKU, p.MinStockLevel, Stock = p.Inventories.Sum(i => (int?)i.Quantity) ?? 0 })
            .ToListAsync(ct);

        // Csak a küszöb átlépésekor szólunk (előtte még a minimum fölött volt), hogy ne jöjjön levél minden kiszállításnál
        var crossed = products
            .Where(p => p.Stock <= p.MinStockLevel && p.Stock + decreaseByProduct[p.Id] > p.MinStockLevel)
            .ToList();
        if (crossed.Count == 0)
            return;

        var recipients = await StaffWithPageAsync(AppPages.Products, ct);
        var table = EmailTemplates.Table(
            ["Termék", "SKU", "Készlet", "Minimum"],
            crossed.Select(p => (IReadOnlyList<string>)[p.Name, p.SKU, $"{p.Stock} db", $"{p.MinStockLevel} db"]),
            2, 3);
        var body = EmailTemplates.Layout(
            "Alacsony készlet",
            ["Az alábbi termékek készlete a minimum szint alá csökkent, érdemes utánrendelni:"],
            table,
            $"{_options.AppBaseUrl}/products", "Termékek megnyitása");
        _queue.Enqueue(new EmailMessage(recipients, $"Alacsony készlet: {string.Join(", ", crossed.Select(p => p.Name))}", body));
    }, "alacsony készlet");

    private string OrderItemsTable(CustomerOrderData order) => EmailTemplates.Table(
        ["Termék", "Mennyiség", "Egységár", "Összesen"],
        order.Items.Select(i => (IReadOnlyList<string>)
            [i.ProductName, $"{i.Quantity} db", EmailTemplates.Money(i.ProductPrice), EmailTemplates.Money(i.ProductPrice * i.Quantity)]),
        1, 2, 3);

    // Azok a felhasználók (e-mail címmel), akiknek valamelyik szerepköre eléri az oldalt
    private async Task<IReadOnlyList<string>> StaffWithPageAsync(string page, CancellationToken ct)
    {
        return await _context.Users
            .Where(u => u.Email != null && u.Email != "")
            .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && _context.RoleClaims.Any(rc =>
                rc.RoleId == ur.RoleId && rc.ClaimType == AppPages.ClaimType && rc.ClaimValue == page)))
            .Select(u => u.Email!)
            .Distinct()
            .ToListAsync(ct);
    }

    private async Task SafeAsync(Func<Task> action, string what)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Nem sikerült összeállítani az értesítést ({What})", what);
        }
    }
}
