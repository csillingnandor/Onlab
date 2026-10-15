using FluentValidation;
using IOMS.BLL.Auth;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok (ismert oldalak és műveletek); a név egyediségét a RoleService ellenőrzi.
public class SaveRoleDataValidator : AbstractValidator<SaveRoleData>
{
    public SaveRoleDataValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(50)
            .WithName("Név");

        RuleForEach(r => r.Pages)
            .Must(page => AppPages.All.Contains(page))
            .WithMessage("Ismeretlen oldal: {PropertyValue}");

        RuleForEach(r => r.Actions)
            .Must(action => AppActions.All.Contains(action))
            .WithMessage("Ismeretlen művelet: {PropertyValue}");

        // Művelet csak az oldalával együtt engedélyezhető (az ismeretlen műveletet az előző szabály jelzi)
        RuleForEach(r => r.Actions)
            .Must((role, action) => !AppActions.All.Contains(action) || role.Pages.Contains(AppActions.PageOf(action)))
            .WithMessage("A(z) {PropertyValue} művelethez az oldalt is engedélyezni kell.");
    }
}
