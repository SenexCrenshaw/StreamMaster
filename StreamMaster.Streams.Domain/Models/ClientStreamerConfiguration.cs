using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ClientStreamerConfiguration(
    SMChannel smChannel,
    int streamGroupNumber,
    string clientUserAgent,
    string clientIPAddress,
    HttpResponse response,
    CancellationToken cancellationToken
    ) : IClientStreamerConfiguration
{


    public HttpResponse Response => response;
    public int StreamGroupNumber { get; set; } = streamGroupNumber;
    public string HttpContextId => Response.HttpContext.TraceIdentifier;

    //Buffering
    public IClientReadStream? ClientStream { get; set; }

    public CancellationToken ClientCancellationToken => cancellationToken;

    public string ClientIPAddress { get; set; } = clientIPAddress;

    public Guid ClientId { get; set; } = Guid.NewGuid();
    public string ClientUserAgent { get; set; } = clientUserAgent;

    //Current Streaming info
    public SMChannel SMChannel { get; set; } = smChannel;
}