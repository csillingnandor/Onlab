using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IOMS.BLL.Notifications;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}

// MailKit alapú küldés: SMTP-n, vagy (Host nélkül, PickupDirectory esetén) .eml fájlba
internal class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailOptions> options, ILogger<EmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("E-mail küldés kikapcsolva, kimaradt: {Subject} -> {To}", message.Subject, string.Join(", ", message.To));
            return;
        }

        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        foreach (var to in message.To)
            mime.To.Add(MailboxAddress.Parse(to));
        mime.Subject = message.Subject;
        mime.Body = new BodyBuilder { HtmlBody = message.HtmlBody }.ToMessageBody();

        if (string.IsNullOrWhiteSpace(_options.Host) && !string.IsNullOrWhiteSpace(_options.PickupDirectory))
        {
            Directory.CreateDirectory(_options.PickupDirectory);
            var path = Path.Combine(_options.PickupDirectory, $"{DateTime.Now:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.eml");
            await mime.WriteToAsync(path, ct);
            _logger.LogInformation("E-mail fájlba mentve: {Path} ({Subject})", path, message.Subject);
            return;
        }

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port,
            _options.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto, ct);
        if (!string.IsNullOrEmpty(_options.UserName))
            await client.AuthenticateAsync(_options.UserName, _options.Password ?? string.Empty, ct);
        await client.SendAsync(mime, ct);
        await client.DisconnectAsync(true, ct);
        _logger.LogInformation("E-mail elküldve: {Subject} -> {To}", message.Subject, string.Join(", ", message.To));
    }
}
