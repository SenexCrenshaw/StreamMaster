using System;

namespace StreamMaster.Ring.API.Exceptions
{
    /// <summary>
    /// Exception thrown when trying to retrieve information regarding a specific Ring device which could not be found
    /// </summary>
    public class DeviceUnknownException : Exception
    {
        /// <summary>
        /// The error message to return
        /// </summary>
        private const string errorMessage = "The Ring device with Id '{0}' could not be found";

        public DeviceUnknownException() : base(string.Format(errorMessage, "unknown"))
        {
        }

        public DeviceUnknownException(int? ringDeviceId) : base(string.Format(errorMessage, ringDeviceId.HasValue ? ringDeviceId.Value.ToString() : "unknown"))
        {
        }

        public DeviceUnknownException(int? ringDeviceId, System.Net.WebException innerException) : base(string.Format(errorMessage, ringDeviceId.HasValue ? ringDeviceId.Value.ToString() : "unknown"), innerException)
        {
        }
    }
}
