namespace IOMS.BLL.Exceptions;

// A művelet ütközik az adatok aktuális állapotával (pl. rendelésben szereplő termék törlése);
// az API 409-es ValidationProblem-ként adja vissza.
public class ConflictException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ConflictException(string field, string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { [field] = [message] };
    }
}
