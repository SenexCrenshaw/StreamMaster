namespace StreamMaster.Domain.Exceptions;

[Serializable]
public class APIException : Exception
{

    public APIException(string message) : base(message)
    {
    }
}
