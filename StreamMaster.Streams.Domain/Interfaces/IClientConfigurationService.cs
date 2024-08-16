using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Interfaces
{
    /// <summary>
    /// Provides methods to create and manage client configurations.
    /// </summary>
    public interface IClientConfigurationService
    {
        /// <summary>
        /// Creates a new instance of <see cref="IClientConfiguration"/> with the specified parameters.
        /// </summary>
        /// <param name="uniqueRequestId">The unique request identifier for the client.</param>
        /// <param name="smChannel">The SMChannel associated with the client.</param>
        /// <param name="clientUserAgent">The client's user agent string.</param>
        /// <param name="clientIPAddress">The client's IP address.</param>
        /// <param name="response">The HTTP response associated with the client.</param>
        /// <param name="cancellationToken">The cancellation token for the client.</param>
        /// <returns>A new instance of <see cref="IClientConfiguration"/>.</returns>
        IClientConfiguration NewClientConfiguration(string uniqueRequestId, SMChannelDto smChannel, string clientUserAgent, string clientIPAddress, HttpResponse response, CancellationToken cancellationToken);
    }
}
