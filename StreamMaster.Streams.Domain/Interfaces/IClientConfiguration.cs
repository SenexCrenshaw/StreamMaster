using Microsoft.AspNetCore.Http;
namespace StreamMaster.Streams.Domain.Interfaces;

public interface IClientConfiguration
{
    void SetUniqueRequestId(string uniqueRequestId);
    CancellationToken ClientCancellationToken { get; }
    string HttpContextId { get; }
    SMChannelDto SMChannel { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ClientStream { get; set; }
    HttpResponse Response { get; }
    string UniqueRequestId { get; }
    int StreamGroupProfileId { get; }
    //int StreamGroupId { get; }
}