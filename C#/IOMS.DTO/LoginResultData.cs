namespace IOMS.DTO;

public class LoginResultData
{
    // JWT; a kliens az Authorization: Bearer fejlécben küldi vissza
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public CurrentUserData User { get; set; } = new();
}
