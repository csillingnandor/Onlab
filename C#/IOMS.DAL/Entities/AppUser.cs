using Microsoft.AspNetCore.Identity;

namespace IOMS.DAL.Entities;

// Bejelentkező felhasználó; a szerepkörök (csoportok) és a jelszó kezelése az Identity táblákban.
// Adatbázis-leképezés: Configurations/AppUserConfiguration.cs
public class AppUser : IdentityUser
{
    // A fejlécben megjelenő teljes név (a UserName a bejelentkezési azonosító)
    public string DisplayName { get; set; } = string.Empty;
}
