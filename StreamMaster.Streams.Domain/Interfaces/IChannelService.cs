namespace StreamMaster.Streams.Domain.Interfaces;
public interface IChannelService
{
    Task CloseChannelAsync(IChannelStatus channelStatus, bool force = false);
    List<IClientConfiguration> GetClientStreamerConfigurations();
    Task CheckForEmptyChannelsAsync(CancellationToken cancellationToken = default);
    List<IChannelStatus> GetChannelStatusFromStreamUrl(string videoUrl);
    Task<IClientConfiguration?> GetClientStreamerConfiguration(string UniqueRequestId, CancellationToken cancellationToken = default);
    Task<bool> SwitchChannelToNextStream(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
    void Dispose();
    IChannelStatus? GetChannelStatus(int smChannelId);
    List<IChannelStatus> GetChannelStatuses();
    IChannelStatus? GetChannelStatusFromSMChannelId(int smChannelId);
    List<IChannelStatus> GetChannelStatusesFromSMStreamId(string smStreamId);
    int GetGlobalStreamsCount();
    bool HasChannel(int SMChannelId);
    Task<IChannelStatus?> GetChannelStatusAsync(IClientConfiguration config);
    Task<bool> UnRegisterClient(string UniqueRequestId, CancellationToken cancellationToken = default);

    Task<IChannelStatus?> SetupChannel(SMChannelDto smChannel);

}