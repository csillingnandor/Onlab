using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IOMS.BLL.Notifications;

// A sorba tett levelek egyenkénti kiküldése. Egy sikertelen küldés csak naplózódik, a többi levelet nem akasztja meg.
internal class EmailBackgroundService : BackgroundService
{
    private readonly IEmailQueue _queue;
    private readonly IEmailSender _sender;
    private readonly ILogger<EmailBackgroundService> _logger;

    public EmailBackgroundService(IEmailQueue queue, IEmailSender sender, ILogger<EmailBackgroundService> logger)
    {
        _queue = queue;
        _sender = sender;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in _queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                await _sender.SendAsync(message, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Nem sikerült elküldeni az e-mailt: {Subject}", message.Subject);
            }
        }
    }
}
