using IOMS.DTO;

namespace IOMS.BLL.Services;

public interface IRoleService
{
    Task<IReadOnlyList<RoleData>> GetAllAsync(CancellationToken ct = default);
    Task<RoleData?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<RoleData> CreateAsync(SaveRoleData data, CancellationToken ct = default);

    // null, ha nincs ilyen szerepkör
    Task<RoleData?> ModifyAsync(string id, SaveRoleData data, CancellationToken ct = default);
    Task<bool> DeleteAsync(string id, CancellationToken ct = default);
}
