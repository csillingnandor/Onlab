using IOMS.API.Infrastructure;
using IOMS.BLL.Auth;
using IOMS.BLL.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequirePage(AppPages.Users)]
public class NotificationsController : ControllerBase
{
    private readonly IDailySummaryService _dailySummary;

    public NotificationsController(IDailySummaryService dailySummary)
    {
        _dailySummary = dailySummary;
    }

    // POST api/notifications/daily-summary?day=2026-10-14: az összefoglaló azonnali kiküldése (alapból a tegnapi napról),
    // pl. kipróbáláshoz. A válasz a címzettek száma.
    [HttpPost("daily-summary")]
    public async Task<ActionResult<object>> SendDailySummary([FromQuery] DateOnly? day, CancellationToken ct)
    {
        var recipients = await _dailySummary.SendAsync(day ?? DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), ct);
        return Ok(new { recipients });
    }
}
