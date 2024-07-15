namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when functionality is called which requires the session to be authenticated while it isn't yet
    /// </summary>
    public class SessionNotAuthenticatedException : Exception
    {
        public SessionNotAuthenticatedException() : base("This session has not yet been authenticated. Please call Authenticate() first.")
        {
        }

        protected SessionNotAuthenticatedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public SessionNotAuthenticatedException(string? message) : base(message)
        {
        }

        public SessionNotAuthenticatedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
