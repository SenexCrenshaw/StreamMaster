using MessagePack;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ClientStreamerConfiguration : IClientStreamerConfiguration
{
    private readonly HttpResponse response;
    private readonly CancellationToken cancellationToken;

    public ClientStreamerConfiguration(
        SMChannelDto smChannel,
        int streamGroupNumber,
        string clientUserAgent,
        string clientIPAddress,
        HttpResponse response,
        CancellationToken cancellationToken
    )
    {
        this.response = response;
        this.cancellationToken = cancellationToken;
        StreamGroupNumber = streamGroupNumber;
        ClientIPAddress = clientIPAddress;
        ClientUserAgent = clientUserAgent;
        SMChannel = smChannel;
    }
    public ClientStreamerConfiguration() { }

    [IgnoreMember]
    public HttpResponse Response => response;
    public int StreamGroupNumber { get; set; }

    [IgnoreMember]
    public string HttpContextId => Response.HttpContext.TraceIdentifier;

    //Buffering
    [IgnoreMember]
    public IClientReadStream? ClientStream { get; set; }

    [IgnoreMember]
    public CancellationToken ClientCancellationToken => cancellationToken;

    public string ClientIPAddress { get; set; }

    public Guid ClientId { get; set; } = Guid.NewGuid();
    public string ClientUserAgent { get; set; }

    //Current Streaming info
    public SMChannelDto SMChannel { get; set; }
}