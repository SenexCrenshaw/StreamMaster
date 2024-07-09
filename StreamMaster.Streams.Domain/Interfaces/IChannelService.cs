namespace StreamMaster.Streams.Domain.Interfaces;
public interface IChannelService
{
    Task<bool> SwitchChannelToNextStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
    void Dispose();
    IChannelStatus? GetChannelStatus(int smChannelId);
    List<IChannelStatus> GetChannelStatuses();
    IChannelStatus? GetChannelStatusFromSMChannelId(int smChannelId);
    List<IChannelStatus> GetChannelStatusesFromSMStreamId(string smStreamId);
    int GetGlobalStreamsCount();
    bool HasChannel(int SMChannelId);
    Task<IChannelStatus?> RegisterChannel(ClientStreamerConfiguration config);

    Task<IChannelStatus?> SetupChannel(SMChannelDto smChannel);
    Task<bool> SetNextChildVideoStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
    void UnRegisterChannel(int smChannelId);
}