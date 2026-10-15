using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; a jelszóerősséget, a név egyediségét és a szerepkörök létezését a UserService (Identity) ellenőrzi.
public class CreateUserDataValidator : AbstractValidator<CreateUserData>
{
    public CreateUserDataValidator()
    {
        RuleFor(u => u.UserName)
            .NotEmpty()
            .MaximumLength(50)
            .WithName("Felhasználónév");

        RuleFor(u => u.DisplayName)
            .NotEmpty()
            .MaximumLength(100)
            .WithName("Teljes név");

        RuleFor(u => u.Password)
            .NotEmpty()
            .WithName("Jelszó");
    }
}
