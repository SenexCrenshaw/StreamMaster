using System.Text.Json.Serialization;

using MessagePack;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams;

/// <summary>
/// Represents the configuration for a client, including details such as the client's IP address, user agent, and associated stream channel.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ClientConfiguration"/> class with the specified parameters.
/// </remarks>
/// <param name="uniqueRequestId">The unique request identifier.</param>
/// <param name="smChannel">The SMChannel associated with the client.</param>
/// <param name="clientUserAgent">The client's user agent string.</param>
/// <param name="clientIPAddress">The client's IP address.</param>
/// <param name="response">The HTTP response associated with the client.</param>
/// <param name="loggerFactory">The logger factory for creating loggers.</param>
/// <param name="cancellationToken">The cancellation Token associated with the client.</param>
public class ClientConfiguration(
    string uniqueRequestId,
    SMChannelDto smChannel,
    string clientUserAgent,
    string clientIPAddress,
    HttpResponse response,
    ILoggerFactory loggerFactory,
    CancellationToken cancellationToken) : IClientConfiguration
{
    public event EventHandler? ClientStopped;
    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConfiguration"/> class for serialization purposes.
    /// </summary>
    //public ClientConfiguration() { }

    /// <inheritdoc/>
    [IgnoreMember]
    public HttpResponse Response { get; } = response;

    /// <inheritdoc/>
    public void SetUniqueRequestId(string uniqueRequestId)
    {
        UniqueRequestId = uniqueRequestId;
    }

    protected virtual void OnClientStopped(EventArgs e)
    {
        ClientStopped?.Invoke(this, e);
    }

    public void Stop()
    {
        ClientStream?.Write([0], 0, 1);
        ClientStream?.Cancel();
        ClientStream?.Flush();
        ClientStream?.Dispose();
        //  Response.CompleteAsync().Wait();
        OnClientStopped(EventArgs.Empty);
    }

    public ILoggerFactory LoggerFactory { get; set; } = loggerFactory;

    /// <inheritdoc/>
    [IgnoreMember]
    public string HttpContextId => Response.HttpContext.TraceIdentifier;

    /// <inheritdoc/>
    [IgnoreMember]
    [JsonIgnore]
    public IClientReadStream? ClientStream { get; set; } = new ClientReadStream(loggerFactory, uniqueRequestId);

    /// <inheritdoc/>
    [IgnoreMember]
    [JsonIgnore]
    public CancellationToken ClientCancellationToken { get; set; } = cancellationToken;

    /// <inheritdoc/>
    public string UniqueRequestId { get; set; } = uniqueRequestId;

    /// <inheritdoc/>
    public string ClientIPAddress { get; set; } = clientIPAddress;

    /// <inheritdoc/>
    public string ClientUserAgent { get; set; } = clientUserAgent;

    /// <inheritdoc/>
    public SMChannelDto SMChannel { get; set; } = smChannel;
}
