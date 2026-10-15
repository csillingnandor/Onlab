namespace IOMS.DTO;

// A bejelentkezett felhasználó a tokenben szereplő adatok alapján
public class CurrentUserData
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = [];

    // A szerepkörei alapján elérhető oldalak (IOMS.BLL.Auth.AppPages kulcsai)
    public IReadOnlyList<string> Pages { get; set; } = [];

    // Az oldalakon engedélyezett műveletek (IOMS.BLL.Auth.AppActions kulcsai, pl. "customer-orders.cancel")
    public IReadOnlyList<string> Actions { get; set; } = [];
}
