using System.Globalization;
using System.Net;
using System.Text;

namespace IOMS.BLL.Notifications;

// Egyszerű, beágyazott stílusú HTML levelek (a levelezők nagy része nem kezel külső CSS-t)
internal static class EmailTemplates
{
    private static readonly CultureInfo Hu = CultureInfo.GetCultureInfo("hu-HU");

    public static string Money(decimal amount) => amount.ToString("#,##0", Hu) + " Ft";

    public static string Date(DateTime utc) => utc.ToLocalTime().ToString("yyyy. MM. dd. HH:mm", Hu);

    public static string Encode(string text) => WebUtility.HtmlEncode(text);

    // Teljes levél: cím, bevezető bekezdések, opcionális táblázat és gomb a felülethez
    public static string Layout(string title, IEnumerable<string> paragraphs, string? tableHtml = null,
        string? buttonUrl = null, string? buttonLabel = null)
    {
        var sb = new StringBuilder();
        sb.Append("<div style=\"font-family:Segoe UI,Arial,sans-serif;max-width:640px;margin:0 auto;color:#212529\">");
        sb.Append("<div style=\"background:#0d6efd;color:#fff;padding:16px 24px;font-size:18px;font-weight:600\">IOMS – ")
          .Append(Encode(title)).Append("</div>");
        sb.Append("<div style=\"padding:16px 24px;border:1px solid #dee2e6;border-top:none\">");
        foreach (var p in paragraphs)
            sb.Append("<p style=\"margin:0 0 12px\">").Append(p).Append("</p>");
        if (tableHtml is not null)
            sb.Append(tableHtml);
        if (buttonUrl is not null)
            sb.Append("<p style=\"margin:20px 0 0\"><a href=\"").Append(Encode(buttonUrl))
              .Append("\" style=\"background:#0d6efd;color:#fff;padding:8px 16px;border-radius:4px;text-decoration:none\">")
              .Append(Encode(buttonLabel ?? "Megnyitás")).Append("</a></p>");
        sb.Append("</div><p style=\"color:#6c757d;font-size:12px;padding:8px 24px\">Ezt a levelet az IOMS rendszer küldte automatikusan.</p></div>");
        return sb.ToString();
    }

    // Táblázat fejléccel; a cellák szövegét kódolja, a jobbra igazítandó oszlopok indexe megadható
    public static string Table(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows, params int[] rightAligned)
    {
        var sb = new StringBuilder("<table style=\"border-collapse:collapse;width:100%;font-size:14px\"><tr>");
        for (var i = 0; i < headers.Count; i++)
            sb.Append(Cell(headers[i], rightAligned.Contains(i), header: true));
        sb.Append("</tr>");
        foreach (var row in rows)
        {
            sb.Append("<tr>");
            for (var i = 0; i < row.Count; i++)
                sb.Append(Cell(row[i], rightAligned.Contains(i), header: false));
            sb.Append("</tr>");
        }
        return sb.Append("</table>").ToString();
    }

    private static string Cell(string text, bool right, bool header)
    {
        var tag = header ? "th" : "td";
        var style = $"padding:6px 8px;border-bottom:1px solid #dee2e6;text-align:{(right ? "right" : "left")}"
            + (header ? ";background:#f8f9fa" : string.Empty);
        return $"<{tag} style=\"{style}\">{Encode(text)}</{tag}>";
    }
}
