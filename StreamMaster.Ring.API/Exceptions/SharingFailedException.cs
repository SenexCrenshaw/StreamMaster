namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when sharing a recording failed
    /// </summary>
    public class SharingFailedException : Exception
    {
        /// <summary>
        /// The error message to return
        /// </summary>
        private const string errorMessage = "Sharing of the recorded Ring event '{0}' failed";

        public SharingFailedException(string id) : base(string.Format(errorMessage, id))
        {
        }

        public SharingFailedException(string id, System.Net.WebException innerException) : base(string.Format(errorMessage, id), innerException)
        {
        }

        public SharingFailedException() : base()
        {
        }

        protected SharingFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public SharingFailedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
