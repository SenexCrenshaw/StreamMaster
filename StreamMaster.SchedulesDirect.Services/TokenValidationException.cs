namespace StreamMaster.SchedulesDirect.Services;

public class TokenValidationException : Exception
{
    public TokenValidationException(string message) : base(message) { }

    public TokenValidationException() : base()
    {
    }

    public TokenValidationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

