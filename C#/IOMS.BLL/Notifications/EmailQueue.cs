using System.Threading.Channels;

namespace IOMS.BLL.Notifications;

public record EmailMessage(IReadOnlyList<string> To, string Subject, string HtmlBody);

// A kérések csak sorba teszik a leveleket, a küldést az EmailBackgroundService végzi,
// így egy lassú vagy elérhetetlen SMTP-szerver nem lassítja / buktatja el a rendelés mentését.
public interface IEmailQueue
{
    void Enqueue(EmailMessage message);
    IAsyncEnumerable<EmailMessage> ReadAllAsync(CancellationToken ct);
}

internal class EmailQueue : IEmailQueue
{
    private readonly Channel<EmailMessage> _channel = Channel.CreateUnbounded<EmailMessage>();

    public void Enqueue(EmailMessage message)
    {
        if (message.To.Count > 0)
            _channel.Writer.TryWrite(message);
    }

    public IAsyncEnumerable<EmailMessage> ReadAllAsync(CancellationToken ct) => _channel.Reader.ReadAllAsync(ct);
}
