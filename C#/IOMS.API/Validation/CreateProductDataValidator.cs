using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; az SKU egyediségét (adatbázist igényel) a ProductService ellenőrzi.
public class CreateProductDataValidator : AbstractValidator<CreateProductData>
{
    public CreateProductDataValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithName("Név");

        RuleFor(p => p.SKU)
            .NotEmpty()
            .MaximumLength(50)
            .WithName("SKU");

        RuleFor(p => p.Category)
            .MaximumLength(50)
            .WithName("Kategória");

        RuleFor(p => p.MinStockLevel)
            .GreaterThanOrEqualTo(0)
            .WithName("Minimális készlet");

        // Az adatbázisban decimal(18,2): legfeljebb 2 tizedesjegy
        RuleFor(p => p.Price)
            .InclusiveBetween(0, 999_999_999)
            .PrecisionScale(18, 2, ignoreTrailingZeros: true)
            .WithName("Ár");
    }
}
