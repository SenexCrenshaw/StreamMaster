namespace StreamMaster.Domain.Exceptions;

[Serializable]
public class APIException : Exception
{

    public APIException(string message) : base(message)
    {
    }

    public APIException() : base()
    {
    }

    public APIException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
