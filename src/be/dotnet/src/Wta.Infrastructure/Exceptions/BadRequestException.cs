namespace Wta.Infrastructure.Exceptions;

public class BadRequestException : Exception
{
    public int Code { get; set; }

    public BadRequestException(string? message = null, int code = 400) : base(message)
    {
        Code = code;
    }
}
