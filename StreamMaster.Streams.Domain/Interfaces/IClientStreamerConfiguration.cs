namespace StreamMaster.Streams.Domain.Interfaces;

public interface IClientStreamerConfiguration
{
    string HttpContextId { get; }
    Task CancelClient(bool includeResponse = true);
    string ChannelVideoStreamId { get; set; }
    string ChannelName { get; set; }
    Guid ClientId { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ReadBuffer { get; set; }
    CancellationTokenSource ClientMasterToken { get; set; }
    string VideoStreamName { get; set; }
}