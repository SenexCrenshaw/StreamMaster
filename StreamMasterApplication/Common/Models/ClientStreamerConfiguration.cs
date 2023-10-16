namespace StreamMasterApplication.Common.Models;

/// <summary>
/// Defines the configuration options for a video streamer.
/// </summary>
public class ClientStreamerConfiguration
{
    public ClientStreamerConfiguration(string videoStreamId, string clientUserAgent, string clientIPAddress, CancellationToken cancellationToken)
    {
        ClientHTTPRequestCancellationToken = cancellationToken;
        ClientIPAddress = clientIPAddress;
        ClientId = Guid.NewGuid();
        ClientUserAgent = clientUserAgent;
        VideoStreamId = videoStreamId;
        VideoStreamName = string.Empty;
        ClientCancellationTokenSource = new();
        ClientMasterToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ClientCancellationTokenSource.Token);
    }

    //Buffering
    public IRingBufferReadStream? ReadBuffer { get; set; }

    //Tokens
    private CancellationToken ClientHTTPRequestCancellationToken { get; }
    private CancellationTokenSource ClientCancellationTokenSource { get; }
    public CancellationTokenSource ClientMasterToken;

    //Client Information
    public string ClientIPAddress { get; set; }
    public Guid ClientId { get; set; }
    public string ClientUserAgent { get; set; }

    //Current Streaming info
    public string VideoStreamId { get; set; }

    public string VideoStreamName { get; set; }
}
