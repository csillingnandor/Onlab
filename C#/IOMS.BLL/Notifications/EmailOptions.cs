namespace IOMS.BLL.Notifications;

// appsettings "Email" szekció. Ha a Host meg van adva, a levelek a megadott SMTP-szerveren mennek ki
// (fejlesztéshez: Ethereal, a hozzáférés user-secrets-ben); egyébként, ha a PickupDirectory meg van adva,
// .eml fájlként ebbe a mappába kerülnek (SMTP-szerver nélkül is megnézhetők).
public class EmailOptions
{
    public const string SectionName = "Email";

    // false: semmilyen levél nem megy ki (csak naplózás)
    public bool Enabled { get; set; } = true;

    public string FromAddress { get; set; } = "ioms@example.com";
    public string FromName { get; set; } = "IOMS";

    public string? PickupDirectory { get; set; }

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public bool UseStartTls { get; set; } = true;
    public string? UserName { get; set; }
    public string? Password { get; set; }

    // A napi összefoglaló küldési ideje (helyi idő, "HH:mm"); üres: nincs ütemezett összefoglaló
    public string? DailySummaryTime { get; set; } = "07:00";

    // A levelekben szereplő hivatkozások alapja (a frontend címe)
    public string AppBaseUrl { get; set; } = "http://localhost:5173";
}
