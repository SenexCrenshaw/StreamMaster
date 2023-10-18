namespace StreamMasterApplication.Common.Interfaces;

public interface IClientStreamerConfiguration
{
    string ChannelVideoStreamId { get; set; }
    Guid ClientId { get; set; }
    string ClientIPAddress { get; set; }
    string ClientUserAgent { get; set; }
    IRingBufferReadStream? ReadBuffer { get; set; }
}