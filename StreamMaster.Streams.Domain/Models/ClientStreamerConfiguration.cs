using MessagePack;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ClientStreamerConfiguration : IClientStreamerConfiguration
{
    public ClientStreamerConfiguration(
        string uniqueRequestId,
        SMChannelDto smChannel,
        int streamGroupId,
         int streamGroupProfileId,
        string clientUserAgent,
        string clientIPAddress,
        HttpResponse response,
        CancellationToken cancellationToken
    )
    {
        UniqueRequestId = uniqueRequestId;
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

    [IgnoreMember]
    public IClientReadStream? ClientStream { get; set; }

    [IgnoreMember]
    public CancellationToken ClientCancellationToken { get; }

    public string UniqueRequestId { get; set; }
    public string ClientIPAddress { get; set; }


    public string ClientUserAgent { get; set; }

    public int StreamGroupProfileId { get; set; }
    //Current Streaming info
    public SMChannelDto SMChannel { get; set; }
}