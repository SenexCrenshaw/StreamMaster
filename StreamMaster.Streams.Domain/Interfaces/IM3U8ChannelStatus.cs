

namespace StreamMaster.Streams.Domain.Interfaces;


public interface IM3U8ChannelStatus : IStreamStatus
{

    string M3U8File { get; }
    string M3U8Directory { get; }
}
