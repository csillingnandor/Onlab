using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

public class LoginDataValidator : AbstractValidator<LoginData>
{
    public LoginDataValidator()
    {
        RuleFor(l => l.UserName).NotEmpty().WithName("Felhasználónév");
        RuleFor(l => l.Password).NotEmpty().WithName("Jelszó");
    }
}
