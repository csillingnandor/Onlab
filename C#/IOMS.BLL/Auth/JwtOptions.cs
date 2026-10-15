namespace IOMS.BLL.Auth;

// appsettings "Jwt" szekció; a Key legalább 32 karakteres titok (fejlesztéshez az appsettings.Development.json-ben)
public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "IOMS";
    public string Audience { get; set; } = "IOMS";
    public string Key { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 480;
}
