using MessagePack;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ClientStreamerConfiguration : IClientStreamerConfiguration
{
    public ClientStreamerConfiguration(
        SMChannelDto smChannel,
        int streamGroupId,
         int streamGroupProfileId,
        string clientUserAgent,
        string clientIPAddress,
        HttpResponse response,
        CancellationToken cancellationToken
    )
    {
        Response = response;
        ClientCancellationToken = cancellationToken;
        StreamGroupId = streamGroupId;
        ClientIPAddress = clientIPAddress;
        ClientUserAgent = clientUserAgent;
        StreamGroupProfileId = streamGroupProfileId;
        SMChannel = smChannel;
    }
    public ClientStreamerConfiguration() { }

    [IgnoreMember]
    public HttpResponse Response { get; }
    public int StreamGroupId { get; set; }

    [IgnoreMember]
    public string HttpContextId => Response.HttpContext.TraceIdentifier;

    //Buffering
    [IgnoreMember]
    public IClientReadStream? ClientStream { get; set; }

    [IgnoreMember]
    public CancellationToken ClientCancellationToken { get; }

    public string ClientIPAddress { get; set; }

    public Guid ClientId { get; set; } = Guid.NewGuid();
    public string ClientUserAgent { get; set; }

    public int StreamGroupProfileId { get; set; }
    //Current Streaming info
    public SMChannelDto SMChannel { get; set; }
}