using Microsoft.AspNetCore.Http;

using StreamMaster.Domain.Models;
namespace StreamMaster.Streams.Domain.Interfaces;

public interface IClientStreamerConfiguration
{
    CancellationToken ClientCancellationToken { get; }

    string HttpContextId { get; }
    SMChannel SMChannel { get; set; }
    Guid ClientId { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ClientStream { get; set; }
    HttpResponse Response { get; }
    //CancellationTokenSource ClientMasterToken { get; set; }
    //Task CancelClient(bool includeResponse = true);
}