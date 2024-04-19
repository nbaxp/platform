namespace Wta.Infrastructure.Exceptions;

public class ProblemException : Exception
{
    public int Code { get; set; }

    public ProblemException(string message, Exception? innerException = null, int code = 500) : base(message)
    {
        Code = code;
    }
}
