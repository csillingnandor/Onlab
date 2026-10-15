using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; az új jelszó erősségét és a szerepkörök létezését a UserService (Identity) ellenőrzi.
public class ModifyUserDataValidator : AbstractValidator<ModifyUserData>
{
    public ModifyUserDataValidator()
    {
        RuleFor(u => u.DisplayName)
            .NotEmpty()
            .MaximumLength(100)
            .WithName("Teljes név");

        RuleFor(u => u.Email)
            .MaximumLength(254)
            .EmailAddress()
            .When(u => !string.IsNullOrWhiteSpace(u.Email))
            .WithName("E-mail");
    }
}
