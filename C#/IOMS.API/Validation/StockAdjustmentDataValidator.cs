using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; a raktár létezését és az eltérést a ProductService ellenőrzi.
public class StockAdjustmentDataValidator : AbstractValidator<StockAdjustmentData>
{
    public StockAdjustmentDataValidator()
    {
        RuleFor(a => a.WarehouseId)
            .GreaterThan(0)
            .WithName("Raktár")
            .WithMessage("Válassz raktárat.");

        RuleFor(a => a.CountedQuantity)
            .InclusiveBetween(0, 1_000_000)
            .WithName("Megszámolt mennyiség");

        RuleFor(a => a.Note)
            .NotEmpty()
            .MaximumLength(200)
            .WithName("Indoklás");
    }
}
