using System;

namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when a download from the Ring API failed
    /// </summary>
    public class DownloadFailedException : Exception
    {
        /// <summary>
        /// The error message to return
        /// </summary>
        private const string errorMessage = "Downloading of the recorded Ring event from '{0}' failed";

        public DownloadFailedException(string url) : base(string.Format(errorMessage, url))
        {
        }

        public DownloadFailedException(string url, System.Net.WebException innerException) : base(string.Format(errorMessage, url), innerException)
        {
        }
    }
}
