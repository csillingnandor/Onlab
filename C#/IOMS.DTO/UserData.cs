namespace IOMS.DTO;

public class UserData
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    // Az értesítések és a napi összefoglaló címe; null, ha nincs megadva (ilyenkor nem kap levelet)
    public string? Email { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}
