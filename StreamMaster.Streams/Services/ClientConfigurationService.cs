using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Services
{
    /// <summary>
    /// Provides a service for creating and managing client configurations.
    /// </summary>
    public class ClientConfigurationService(ILoggerFactory loggerFactory) : IClientConfigurationService
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
        public IClientConfiguration NewClientConfiguration(
            string uniqueRequestId,
            SMChannelDto smChannel,
            string clientUserAgent,
            string clientIPAddress,
            HttpResponse response,
            CancellationToken cancellationToken)
        {
            ClientConfiguration config = new(
                uniqueRequestId,
                smChannel,
                clientUserAgent,
                clientIPAddress,
                response,
                loggerFactory,
                cancellationToken);

            config.ClientStream ??= new ClientReadStream(loggerFactory, config.UniqueRequestId);
            return config;
        }
    }
}
