using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; az e-mail egyediségét (adatbázist igényel) a CustomerService ellenőrzi.
public class CreateCustomerDataValidator : AbstractValidator<CreateCustomerData>
{
    public CreateCustomerDataValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithName("Név");

        RuleFor(c => c.Email)
            .NotEmpty()
            .MaximumLength(254)
            .EmailAddress()
            .WithName("E-mail");

        RuleFor(c => c.Phone)
            .MaximumLength(30)
            .Matches(@"^\+?[0-9 ()/-]+$")
            .When(c => !string.IsNullOrEmpty(c.Phone))
            .WithName("Telefon")
            .WithMessage("A telefonszám csak számjegyeket, szóközt és + ( ) / - jeleket tartalmazhat.");
    }
}
