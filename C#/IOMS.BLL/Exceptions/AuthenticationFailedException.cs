namespace IOMS.BLL.Exceptions;

// Sikertelen bejelentkezés (hibás név/jelszó, zárolt fiók); az API 401-es ValidationProblem-ként adja vissza.
public class AuthenticationFailedException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public string InvalidCredentialsMessage = "Hibás felhasználónév vagy jelszó.";
    public string LockOutMessage = "Túl sok sikertelen próbálkozás, próbálja újra később";

    public AuthenticationFailedException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]> { [string.Empty] = [message] };
    }
}
