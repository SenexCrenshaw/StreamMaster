using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces;
public interface IChannelService
{
    Task<bool> SwitchChannelToNextStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
    void Dispose();
    IChannelStatus? GetChannelStatus(int smChannelId);
    List<IChannelStatus> GetChannelStatuses();
    List<IChannelStatus> GetChannelStatusesFromSMChannelId(int smChannelId);
    List<IChannelStatus> GetChannelStatusesFromSMStreamId(string smStreamId);
    int GetGlobalStreamsCount();
    bool HasChannel(int SMChannelId);
    Task<IChannelStatus?> RegisterChannel(IClientStreamerConfiguration config);

    Task<IChannelStatus?> SetupChannel(SMChannel smChannel);
    Task SetNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
    void UnRegisterChannel(int smChannelId);
}