using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace IOMS.API.Validation;

internal static class ValidationResultExtensions
{
    // A ControllerBase.ValidationProblem(ModelState) ebből ugyanolyan 400-as választ ad,
    // mint a beépített modellvalidáció és a BLL üzleti hibái (mezőnként csoportosított hibákkal).
    public static ModelStateDictionary ToModelState(this ValidationResult result)
    {
        var modelState = new ModelStateDictionary();
        foreach (var error in result.Errors)
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);

        return modelState;
    }
}
