using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IClientStreamerConfiguration
{
    string HttpContextId { get; }
    SMChannel SMChannel { get; set; }
    string ChannelName { get; set; }
    Guid ClientId { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ClientStream { get; set; }
    CancellationTokenSource ClientMasterToken { get; set; }
    string VideoStreamName { get; set; }
    Task CancelClient(bool includeResponse = true);
}