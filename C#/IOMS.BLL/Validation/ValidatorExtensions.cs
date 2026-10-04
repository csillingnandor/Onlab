using FluentValidation;
using IOMS.BLL.Exceptions;

namespace IOMS.BLL.Validation;

internal static class ValidatorExtensions
{
    // Lefuttatja a validátort, és hiba esetén ugyanazt a kivételt dobja, mint az üzleti szabályok,
    // így az API egységesen 400-as ValidationProblem választ ad (mezőnként csoportosított hibákkal).
    public static async Task EnsureValidAsync<T>(this IValidator<T> validator, T instance, CancellationToken ct)
    {
        var result = await validator.ValidateAsync(instance, ct);
        if (!result.IsValid)
            throw new BusinessValidationException(result.ToDictionary());
    }
}
