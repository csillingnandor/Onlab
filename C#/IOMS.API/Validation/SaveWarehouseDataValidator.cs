using FluentValidation;
using IOMS.DTO;

namespace IOMS.API.Validation;

// Formai szabályok; a név egyediségét a WarehouseService ellenőrzi.
public class SaveWarehouseDataValidator : AbstractValidator<SaveWarehouseData>
{
    public SaveWarehouseDataValidator()
    {
        RuleFor(w => w.Name)
            .NotEmpty()
            .MaximumLength(100)
            .WithName("Név");

        RuleFor(w => w.Address)
            .MaximumLength(200)
            .WithName("Cím");

        RuleFor(w => w.Capacity)
            .GreaterThanOrEqualTo(0)
            .WithName("Kapacitás");

        RuleFor(w => w.Latitude)
            .InclusiveBetween(-90, 90)
            .WithName("Szélesség");

        RuleFor(w => w.Longitude)
            .InclusiveBetween(-180, 180)
            .WithName("Hosszúság");

        // A térképen csak a teljes koordinátapár jeleníthető meg
        RuleFor(w => w)
            .Must(w => w.Latitude.HasValue == w.Longitude.HasValue)
            .WithName("Koordináták")
            .WithMessage("A szélességet és a hosszúságot együtt kell megadni (vagy egyiket sem).");
    }
}
