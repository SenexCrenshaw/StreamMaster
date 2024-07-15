using System.Net;

namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when the Ring API returns a different response than it was expected to return
    /// </summary>
    public class UnexpectedOutcomeException : Exception
    {
        /// <summary>
        /// The error message to return
        /// </summary>
        private const string errorMessage = "The Ring API returned a different response {0} than was expected {1}";

        /// <summary>
        /// The Http Status code that was returned
        /// </summary>
        public readonly HttpStatusCode ReturnedStatusCode;

        /// <summary>
        /// The Http Status code that was expected to be returned
        /// </summary>
        public readonly HttpStatusCode ExpectedStatusCode;

        public UnexpectedOutcomeException(HttpStatusCode returnedStatusCode, HttpStatusCode expectedStatusCode) : base(string.Format(errorMessage, returnedStatusCode, expectedStatusCode))
        {
            ReturnedStatusCode = returnedStatusCode;
            ExpectedStatusCode = expectedStatusCode;
        }

        public UnexpectedOutcomeException(HttpStatusCode returnedStatusCode, HttpStatusCode expectedStatusCode, WebException innerException) : base(string.Format(errorMessage, returnedStatusCode, expectedStatusCode), innerException)
        {
            ReturnedStatusCode = returnedStatusCode;
            ExpectedStatusCode = expectedStatusCode;
        }

        public UnexpectedOutcomeException() : base()
        {
        }

        protected UnexpectedOutcomeException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }

        public UnexpectedOutcomeException(string? message) : base(message)
        {
        }

        public UnexpectedOutcomeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
