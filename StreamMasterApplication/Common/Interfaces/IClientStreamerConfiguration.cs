namespace StreamMasterApplication.Common.Interfaces;

public interface IClientStreamerConfiguration
{
    string ChannelVideoStreamId { get; set; }
    string ChannelName { get; set; }
    Guid ClientId { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IClientReadStream? ReadBuffer { get; set; }
    CancellationTokenSource ClientMasterToken { get; set; }
}