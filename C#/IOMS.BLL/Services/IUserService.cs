using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserData>> GetAllAsync(CancellationToken ct = default);
    Task<UserData?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<UserData> CreateAsync(CreateUserData data, CancellationToken ct = default);

    // null, ha nincs ilyen felhasználó
    Task<UserData?> ModifyAsync(string id, ModifyUserData data, CancellationToken ct = default);

    // currentUserId: a bejelentkezett felhasználó, aki saját magát nem törölheti
    Task<bool> DeleteAsync(string id, string currentUserId, CancellationToken ct = default);
}
