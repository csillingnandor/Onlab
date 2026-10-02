namespace IOMS.BLL.Exceptions;

// Üzleti szabály sérült (pl. nem létező vevő); az API 400-as ValidationProblem-ként adja vissza.
public class BusinessValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public BusinessValidationException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }
}
