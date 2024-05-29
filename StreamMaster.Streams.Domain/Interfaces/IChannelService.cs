using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;
public interface IChannelService
{
    void Dispose();
    IChannelStatus? GetChannelStatus(int smChannelId);
    List<IChannelStatus> GetChannelStatuses();
    List<IChannelStatus> GetChannelStatusesFromSMChannelId(int smChannelId);
    List<IChannelStatus> GetChannelStatusesFromSMStreamId(string smStreamId);
    int GetGlobalStreamsCount();
    bool HasChannel(int SMChannelId);
    Task<IChannelStatus?> RegisterChannel(SMChannel smChannel, bool fetch = false);
    Task SetNextChildVideoStream(int smChannelId, string? overrideNextVideoStreamId = null);
    void UnRegisterChannel(int smChannelId);
}