using FluentValidation;
using IOMS.API.Validation;
using IOMS.BLL.Services;
using IOMS.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<LoginData> _loginValidator;

    public AuthController(IAuthService authService, IValidator<LoginData> loginValidator)
    {
        _authService = authService;
        _loginValidator = loginValidator;
    }

    // POST api/auth/login: hibás adatoknál 401 (az üzenetet a BLL adja)
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResultData>> Login(LoginData data, CancellationToken ct)
    {
        var validation = await _loginValidator.ValidateAsync(data, ct);
        if (!validation.IsValid)
            return ValidationProblem(validation.ToModelState());

        return Ok(await _authService.LoginAsync(data, ct));
    }

    // GET api/auth/me: a kliens induláskor ezzel ellenőrzi, hogy a tárolt token még érvényes-e
    [HttpGet("me")]
    [Authorize]
    public ActionResult<CurrentUserData> Me()
    {
        return Ok(_authService.GetCurrentUser(User));
    }
}
