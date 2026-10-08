using IOMS.BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace IOMS.API.Infrastructure;

// A BLL kivételeit ugyanolyan ValidationProblem válasszá alakítja, mint a kontrollerek validációja:
// üzleti szabály sérülése 400, ütközés az adatok állapotával 409.
public class BusinessExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public BusinessExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var (status, errors) = exception switch
        {
            BusinessValidationException e => (StatusCodes.Status400BadRequest, e.Errors),
            ConflictException e => (StatusCodes.Status409Conflict, e.Errors),
            _ => (0, null),
        };

        if (errors is null)
            return false;

        httpContext.Response.StatusCode = status;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ValidationProblemDetails(errors)
            {
                Status = status,
            },
            Exception = exception,
        });
    }
}
