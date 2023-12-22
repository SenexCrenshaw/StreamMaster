using Microsoft.AspNetCore.Http;

using StreamMasterApplication.Common.Interfaces;
namespace StreamMasterInfrastructure.VideoStreamManager.Clients;

/// <summary>
/// Defines the configuration options for a video streamer.
/// </summary>
public sealed class ClientStreamerConfiguration : IClientStreamerConfiguration
{

    private readonly HttpResponse response;

    public ClientStreamerConfiguration(
        string channelVideoStreamId,
         string channelName,
        string clientUserAgent,
        string clientIPAddress,
        CancellationToken cancellationToken,
        HttpResponse response)
    {
        this.response = response;
        ClientHTTPRequestCancellationToken = cancellationToken;
        ClientIPAddress = clientIPAddress;
        ClientId = Guid.NewGuid();
        ClientUserAgent = clientUserAgent;
        ChannelVideoStreamId = channelVideoStreamId;
        ChannelName = channelName;
        //ClientCancellationTokenSource = new();
        ClientMasterToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ClientHTTPRequestCancellationToken);
    }

    public string HttpContextId => this.response.HttpContext.TraceIdentifier;

    public async Task CancelClient(bool includeAbort = true)
    {
        if (ReadBuffer != null)
        {
            ReadBuffer.Cancel();
            ReadBuffer.Dispose();
            ReadBuffer = null;
        }

        try
        {
            if (includeAbort)
            {
                if (!response.HasStarted)
                {
                    response.Body.Flush();

                }
                await response.CompleteAsync();
                if (!response.HttpContext.Response.HasStarted)
                {
                    response.HttpContext.Abort();
                }
            }
        }
        catch (ObjectDisposedException ex)
        {
            // Log the exception or handle it as necessary
        }
        catch (Exception ex)
        {

        }
        //ClientCancellationTokenSource.Cancel();

    }

    //Buffering
    public IClientReadStream? ReadBuffer { get; set; }

    //Tokens
    private CancellationToken ClientHTTPRequestCancellationToken { get; }

    //private CancellationTokenSource ClientCancellationTokenSource { get; }
    public CancellationTokenSource ClientMasterToken { get; set; }

    //Client Information
    public string ClientIPAddress { get; set; }

    public Guid ClientId { get; set; }
    public string ClientUserAgent { get; set; }

    //Current Streaming info
    public string ChannelVideoStreamId { get; set; }
    public string ChannelName { get; set; }
}