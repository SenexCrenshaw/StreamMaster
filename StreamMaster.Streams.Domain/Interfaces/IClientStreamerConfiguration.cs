using Microsoft.AspNetCore.Http;
namespace StreamMaster.Streams.Domain.Interfaces;

public interface IClientStreamerConfiguration
{
    CancellationToken ClientCancellationToken { get; }

    string HttpContextId { get; }
    SMChannelDto SMChannel { get; set; }
    Guid ClientId { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ClientStream { get; set; }
    HttpResponse Response { get; }
    //CancellationTokenSource ClientMasterToken { get; set; }
    //Task CancelClient(bool includeResponse = true);
}