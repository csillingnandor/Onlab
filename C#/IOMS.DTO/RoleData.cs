namespace IOMS.DTO;

public class RoleData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Az oldalak (IOMS.BLL.Auth.AppPages kulcsai), amelyeket a szerepkör tagjai elérnek
    public IReadOnlyList<string> Pages { get; set; } = [];

    // Az oldalakon engedélyezett műveletek (IOMS.BLL.Auth.AppActions kulcsai); csak a Pages oldalaihoz tartozhatnak
    public IReadOnlyList<string> Actions { get; set; } = [];

    public int UserCount { get; set; }

    // A rendszer-szerepkör (adminisztrátor) nem nevezhető át, nem törölhető, és mindig minden oldalt és műveletet elér
    public bool IsSystem { get; set; }
}
