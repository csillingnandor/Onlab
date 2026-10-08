using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; a vevő és a termékek létezését (adatbázist igényel) a CustomerOrderService ellenőrzi.
public class CreateCustomerOrderDataValidator : AbstractValidator<CreateCustomerOrderData>
{
    public CreateCustomerOrderDataValidator()
    {
        RuleFor(o => o.CustomerId)
            .GreaterThan(0)
            .WithName("Vevő azonosító");

        RuleFor(o => o.Items)
            .NotEmpty()
            .WithName("Tételek")
            .WithMessage("A rendelésnek legalább egy tételt kell tartalmaznia.");

        RuleForEach(o => o.Items)
            .SetValidator(new CreateCustomerOrderItemDataValidator());
    }
}

public class CreateCustomerOrderItemDataValidator : AbstractValidator<CreateCustomerOrderItemData>
{
    public CreateCustomerOrderItemDataValidator()
    {
        RuleFor(i => i.ProductId)
            .GreaterThan(0)
            .WithName("Termék azonosító");

        RuleFor(i => i.Quantity)
            .InclusiveBetween(1, 10_000)
            .WithName("Mennyiség");
    }
}
