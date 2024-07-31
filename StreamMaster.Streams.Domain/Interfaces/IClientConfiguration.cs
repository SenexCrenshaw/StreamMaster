using Microsoft.AspNetCore.Http;
namespace StreamMaster.Streams.Domain.Interfaces;

public interface IClientConfiguration
{
    CancellationToken ClientCancellationToken { get; }
    string HttpContextId { get; }
    SMChannelDto SMChannel { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ClientStream { get; set; }
    HttpResponse Response { get; }
    string UniqueRequestId { get; }
    int StreamGroupProfileId { get; }
    //CancellationTokenSource ClientMasterToken { get; set; }
    //Task CancelClient(bool includeResponse = true);
}