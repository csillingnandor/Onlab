using FluentValidation;
using IOMS.API.Infrastructure;
using IOMS.API.Validation;
using IOMS.BLL.Auth;
using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequirePage(AppPages.Users)]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly IValidator<SaveRoleData> _saveValidator;

    public RolesController(IRoleService roleService, IValidator<SaveRoleData> saveValidator)
    {
        _roleService = roleService;
        _saveValidator = saveValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _roleService.GetAllAsync(ct));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoleData>> GetById(string id, CancellationToken ct)
    {
        var role = await _roleService.GetByIdAsync(id, ct);
        return role is null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleData>> Create(SaveRoleData data, CancellationToken ct)
    {
        var validation = await _saveValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _roleService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // 409: az adminisztrátor szerepkör nem módosítható
    [HttpPut("{id}")]
    public async Task<ActionResult<RoleData>> Modify(string id, SaveRoleData data, CancellationToken ct)
    {
        var validation = await _saveValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _roleService.ModifyAsync(id, data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    // 409: rendszer-szerepkör, vagy még tartoznak hozzá felhasználók
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        return await _roleService.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}
