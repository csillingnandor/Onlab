using IOMS.BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Infrastructure;

// A BLL kivételeit ugyanolyan ValidationProblem (400) válasszá alakítja, mint a modellvalidáció.
public class BusinessExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public BusinessExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        if (exception is not BusinessValidationException validation)
            return false;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ValidationProblemDetails(validation.Errors)
            {
                Status = StatusCodes.Status400BadRequest,
            },
            Exception = exception,
        });
    }
}
