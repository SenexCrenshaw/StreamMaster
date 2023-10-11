namespace StreamMasterAPI.Exceptions
{
    public class InvalidApiKeyException : Exception
    {
        public InvalidApiKeyException()
        {
        }

        public InvalidApiKeyException(string message)
            : base(message)
        {
        }

        public InvalidApiKeyException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
