using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Services
{
    /// <summary>
    /// Provides a service for creating and managing client configurations.
    /// </summary>
    public class ClientConfigurationService(ILoggerFactory loggerFactory, IOptionsMonitor<Setting> settings) : IClientConfigurationService
    {
        /// <summary>
        /// Creates a new instance of <see cref="IClientConfiguration"/> with the specified parameters.
        /// </summary>
        /// <param name="uniqueRequestId">The unique request identifier for the client.</param>
        /// <param name="smChannel">The SMChannel associated with the client.</param>
        /// <param name="clientUserAgent">The client's user agent string.</param>
        /// <param name="clientIPAddress">The client's IP address.</param>
        /// <param name="response">The HTTP response associated with the client.</param>
        /// <param name="cancellationToken">The cancellation Token for the client.</param>
        /// <returns>A new instance of <see cref="IClientConfiguration"/>.</returns>
        public IClientConfiguration NewClientConfiguration(
            string uniqueRequestId,
            SMChannelDto smChannel,
            string clientUserAgent,
            string clientIPAddress,
            HttpResponse response,
            CancellationToken cancellationToken)
        {
            ClientConfiguration clientConfig = new(
                uniqueRequestId,
                smChannel,
                clientUserAgent,
                clientIPAddress,
                response,
                loggerFactory,
                settings,
                cancellationToken);

            //config.ClientStream ??= new ClientReadStream(loggerFactory, config.UniqueRequestId);
            //config.ClientStream.ClientStreamTimedOut += (sender, e) => config.Stop();
            // Start the task for this client
            _ = Task.Run(clientConfig.StreamFromPipeToResponseAsync, cancellationToken);
            return clientConfig;
        }

        //public IClientConfiguration Copy(IClientConfiguration clientConfiguration)
        //{
        //    return new ClientConfiguration(
        //        clientConfiguration.UniqueRequestId,
        //        clientConfiguration.SMChannel,
        //        clientConfiguration.ClientUserAgent,
        //        clientConfiguration.ClientIPAddress,
        //        clientConfiguration.Response,
        //           clientConfiguration.Pipe,
        //        clientConfiguration.LoggerFactory,
        //        clientConfiguration.ClientCancellationToken);
        //}
    }
}
