using Microsoft.AspNetCore.Http;

using StreamMaster.Streams.Domain.Interfaces;
namespace StreamMaster.Infrastructure.Clients;

public sealed class ClientStreamerConfiguration : IClientStreamerConfiguration
{

    private readonly HttpResponse response;

    public ClientStreamerConfiguration(
        SMChannel smChannel,
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
        SMChannel = smChannel;
        //ClientCancellationTokenSource = new();
        ClientMasterToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ClientHTTPRequestCancellationToken);
    }

    public string HttpContextId => response.HttpContext.TraceIdentifier;

    public async Task CancelClient(bool includeAbort = true)
    {
        if (Stream != null)
        {
            Stream.Channel?.Writer.Complete();
            Stream.Cancel();
            Stream.Dispose();
            Stream = null;
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
    public IClientReadStream? Stream { get; set; }

    //Tokens
    private CancellationToken ClientHTTPRequestCancellationToken { get; }

    //private CancellationTokenSource ClientCancellationTokenSource { get; }
    public CancellationTokenSource ClientMasterToken { get; set; }

    //Client Information
    public string ClientIPAddress { get; set; }

    public Guid ClientId { get; set; }
    public string ClientUserAgent { get; set; }

    //Current Streaming info
    public SMChannel SMChannel { get; set; }
    public string ChannelName { get; set; }
    public string VideoStreamName { get; set; }
}