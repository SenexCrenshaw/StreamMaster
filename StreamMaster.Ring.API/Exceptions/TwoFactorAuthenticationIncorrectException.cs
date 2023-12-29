using System;

namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when the Ring API required two factor authentication and the provided two factor authentication was invalid or expired
    /// </summary>
    public class TwoFactorAuthenticationIncorrectException : Exception
    {
        /// <summary>
        /// The error message to return
        /// </summary>
        private const string errorMessage = "The two factor authentication token provided is invalid or has expired";

        public TwoFactorAuthenticationIncorrectException() : base(errorMessage)
        {
        }

        public TwoFactorAuthenticationIncorrectException(Exception innerException) : base(errorMessage, innerException)
        {
        }
    }
}
