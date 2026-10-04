namespace IOMS.BLL.Exceptions;

// Validációs vagy üzleti szabály sérült (pl. hiányzó név, nem létező vevő);
// az API 400-as ValidationProblem-ként adja vissza.
public class BusinessValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }

    // Több mező hibái egyszerre (pl. a FluentValidation eredményéből)
    public BusinessValidationException(IDictionary<string, string[]> errors)
        : base("Egy vagy több mező értéke hibás.")
    {
        Errors = errors;
    }
}
