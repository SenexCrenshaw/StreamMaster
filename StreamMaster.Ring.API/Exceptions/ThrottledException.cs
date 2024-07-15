namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when the Ring API returns HTTP 429 Too many requests
    /// </summary>
    public class ThrottledException : Exception
    {
        public ThrottledException() : base("The request has been denied by Ring due to too many requests. Try again in a few minutes.")
        {
        }

        public ThrottledException(Exception innerException) : base("The request has been denied by Ring due to too many requests. Try again in a few minutes.", innerException)
        {
        }

        protected ThrottledException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public ThrottledException(string? message) : base(message)
        {
        }

        public ThrottledException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
