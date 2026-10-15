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
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IValidator<CreateUserData> _createValidator;
    private readonly IValidator<ModifyUserData> _modifyValidator;

    public UsersController(
        IUserService userService,
        IAuthService authService,
        IValidator<CreateUserData> createValidator,
        IValidator<ModifyUserData> modifyValidator)
    {
        _userService = userService;
        _authService = authService;
        _createValidator = createValidator;
        _modifyValidator = modifyValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserData>>> GetAll(CancellationToken ct)
    {
        return Ok(await _userService.GetAllAsync(ct));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserData>> GetById(string id, CancellationToken ct)
    {
        var user = await _userService.GetByIdAsync(id, ct);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserData>> Create(CreateUserData data, CancellationToken ct)
    {
        // Formai szabályok; a jelszóerősséget és a név egyediségét az Identity ellenőrzi (UserService).
        var validation = await _createValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var created = await _userService.CreateAsync(data, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserData>> Modify(string id, ModifyUserData data, CancellationToken ct)
    {
        var validation = await _modifyValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        var modified = await _userService.ModifyAsync(id, data, ct);
        return modified is null ? NotFound() : Ok(modified);
    }

    // 409: saját magát vagy az utolsó adminisztrátort nem lehet törölni
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken ct)
    {
        var currentUserId = _authService.GetCurrentUser(User).Id;
        return await _userService.DeleteAsync(id, currentUserId, ct) ? NoContent() : NotFound();
    }
}
