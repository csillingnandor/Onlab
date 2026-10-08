using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; a beszállító és a termékek létezését (adatbázist igényel) a SupplierOrderService ellenőrzi.
public class CreateSupplierOrderDataValidator : AbstractValidator<CreateSupplierOrderData>
{
    public CreateSupplierOrderDataValidator()
    {
        RuleFor(o => o.SupplierId)
            .GreaterThan(0)
            .WithName("Beszállító azonosító");

        RuleFor(o => o.Items)
            .NotEmpty()
            .WithName("Tételek")
            .WithMessage("A rendelésnek legalább egy tételt kell tartalmaznia.");

        RuleForEach(o => o.Items)
            .SetValidator(new CreateSupplierOrderItemDataValidator());
    }
}

public class CreateSupplierOrderItemDataValidator : AbstractValidator<CreateSupplierOrderItemData>
{
    public CreateSupplierOrderItemDataValidator()
    {
        RuleFor(i => i.ProductId)
            .GreaterThan(0)
            .WithName("Termék azonosító");

        RuleFor(i => i.Quantity)
            .InclusiveBetween(1, 10_000)
            .WithName("Mennyiség");

        // Az adatbázisban decimal(18,2): legfeljebb 2 tizedesjegy
        RuleFor(i => i.UnitCost)
            .InclusiveBetween(0, 999_999_999)
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithName("Egységár");
    }
}
