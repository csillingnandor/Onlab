using System.Security.Claims;
using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IAuthService
{
    // Hibás adatok vagy zárolt fiók esetén AuthenticationFailedException
    Task<LoginResultData> LoginAsync(LoginData data, CancellationToken ct = default);

    // A token claim-jeiből (adatbázis nélkül), hogy pontosan azt adja, amit az API is figyelembe vesz
    CurrentUserData GetCurrentUser(ClaimsPrincipal principal);
}
