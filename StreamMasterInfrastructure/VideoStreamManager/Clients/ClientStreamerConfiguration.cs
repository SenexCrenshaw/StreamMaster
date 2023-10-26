using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.VideoStreamManager.Clients;

/// <summary>
/// Defines the configuration options for a video streamer.
/// </summary>
public class ClientStreamerConfiguration : IClientStreamerConfiguration
{
    public ClientStreamerConfiguration(
        string channelVideoStreamId,
        string clientUserAgent,
        string clientIPAddress,
        CancellationToken cancellationToken)
    {
        ClientHTTPRequestCancellationToken = cancellationToken;
        ClientIPAddress = clientIPAddress;
        ClientId = Guid.NewGuid();
        ClientUserAgent = clientUserAgent;
        ChannelVideoStreamId = channelVideoStreamId;
        ClientCancellationTokenSource = new();
        ClientMasterToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ClientHTTPRequestCancellationToken, ClientCancellationTokenSource.Token);
    }

    //Buffering
    public IRingBufferReadStream? ReadBuffer { get; set; }

    //Tokens
    private CancellationToken ClientHTTPRequestCancellationToken { get; }

    private CancellationTokenSource ClientCancellationTokenSource { get; }
    public CancellationTokenSource ClientMasterToken { get; set; }

    //Client Information
    public string ClientIPAddress { get; set; }

    public Guid ClientId { get; set; }
    public string ClientUserAgent { get; set; }

    //Current Streaming info
    public string ChannelVideoStreamId { get; set; }

}