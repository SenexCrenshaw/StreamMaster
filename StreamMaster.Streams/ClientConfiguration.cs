using MessagePack;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams
{
    /// <summary>
    /// Represents the configuration for a client, including details such as the client's IP address, user agent, and associated stream channel.
    /// </summary>
    public sealed class ClientConfiguration : IClientConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConfiguration"/> class with the specified parameters.
        /// </summary>
        /// <param name="uniqueRequestId">The unique request identifier.</param>
        /// <param name="smChannel">The SMChannel associated with the client.</param>
        /// <param name="clientUserAgent">The client's user agent string.</param>
        /// <param name="clientIPAddress">The client's IP address.</param>
        /// <param name="response">The HTTP response associated with the client.</param>
        /// <param name="loggerFactory">The logger factory for creating loggers.</param>
        /// <param name="cancellationToken">The cancellation token associated with the client.</param>
        public ClientConfiguration(
            string uniqueRequestId,
            SMChannelDto smChannel,
            string clientUserAgent,
            string clientIPAddress,
            HttpResponse response,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken)
        {
            UniqueRequestId = uniqueRequestId;
            Response = response;
            ClientCancellationToken = cancellationToken;
            ClientIPAddress = clientIPAddress;
            ClientUserAgent = clientUserAgent;
            SMChannel = smChannel;
            ClientStream = new ClientReadStream(loggerFactory, uniqueRequestId);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientConfiguration"/> class for serialization purposes.
        /// </summary>
        public ClientConfiguration() { }

        /// <inheritdoc/>
        [IgnoreMember]
        public HttpResponse Response { get; }

        /// <inheritdoc/>
        public void SetUniqueRequestId(string uniqueRequestId)
        {
            UniqueRequestId = uniqueRequestId;
        }

        /// <inheritdoc/>
        [IgnoreMember]
        public string HttpContextId => Response.HttpContext.TraceIdentifier;

        /// <inheritdoc/>
        [IgnoreMember]
        public IClientReadStream? ClientStream { get; set; }

        /// <inheritdoc/>
        [IgnoreMember]
        public CancellationToken ClientCancellationToken { get; }

        /// <inheritdoc/>
        public string UniqueRequestId { get; set; }

        /// <inheritdoc/>
        public string ClientIPAddress { get; set; }

        /// <inheritdoc/>
        public string ClientUserAgent { get; set; }

        /// <inheritdoc/>
        public SMChannelDto SMChannel { get; set; }
    }
}
