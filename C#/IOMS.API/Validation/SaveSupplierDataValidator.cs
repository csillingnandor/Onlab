using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; a név egyediségét a SupplierService ellenőrzi.
public class SaveSupplierDataValidator : AbstractValidator<SaveSupplierData>
{
    public SaveSupplierDataValidator()
    {
        RuleFor(s => s.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithName("Név");

        RuleFor(s => s.Email)
            .MaximumLength(254)
            .EmailAddress()
            .When(s => !string.IsNullOrWhiteSpace(s.Email))
            .WithName("E-mail");

        RuleFor(s => s.Phone)
            .MaximumLength(30)
            .Matches(@"^\+?[0-9 ()/-]+$")
            .When(s => !string.IsNullOrWhiteSpace(s.Phone))
            .WithName("Telefon")
            .WithMessage("A telefonszám csak számjegyeket, szóközt és + ( ) / - jeleket tartalmazhat.");
    }
}
