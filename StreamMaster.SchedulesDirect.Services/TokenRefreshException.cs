namespace StreamMaster.SchedulesDirect.Services;

public class TokenRefreshException : Exception
{
    public TokenRefreshException(string message) : base(message) { }

    public TokenRefreshException() : base()
    {
    }

    public TokenRefreshException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

