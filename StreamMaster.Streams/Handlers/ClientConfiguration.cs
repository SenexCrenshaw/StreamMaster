using MessagePack;

using Microsoft.AspNetCore.Http;

namespace StreamMaster.Streams.Handlers;

public sealed class ClientConfiguration : IClientConfiguration
{
    public ClientConfiguration(
        string uniqueRequestId,
        SMChannelDto smChannel,
        int streamGroupId,
         int streamGroupProfileId,
        string clientUserAgent,
        string clientIPAddress,
        HttpResponse response,
        ILoggerFactory loggerFactory,
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
        ClientStream = new ClientReadStream(loggerFactory, uniqueRequestId);
    }
    public ClientConfiguration() { }

    [IgnoreMember]
    public HttpResponse Response { get; }
    public int StreamGroupId { get; set; }

    public void SetUniqueRequestId(string uniqueRequestId)
    {
        UniqueRequestId = uniqueRequestId;
    }
    [IgnoreMember]
    public string HttpContextId => Response.HttpContext.TraceIdentifier;

    [IgnoreMember]
    public IClientReadStream? ClientStream { get; set; }

    [IgnoreMember]
    public CancellationToken ClientCancellationToken { get; }

    public string UniqueRequestId { get; set; }
    public string ClientIPAddress { get; set; }

    public string ClientUserAgent { get; set; }

    public int StreamGroupProfileId { get; }
    //Current Streaming info
    public SMChannelDto SMChannel { get; set; }
}