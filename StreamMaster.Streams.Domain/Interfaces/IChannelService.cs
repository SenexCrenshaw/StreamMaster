namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IChannelService : IDisposable
    {
        Task CheckForEmptyChannelsAsync(CancellationToken cancellationToken = default);
        Task CloseChannelAsync(IChannelStatus channelStatus, bool force = false);
        Task<IClientConfiguration?> GetClientStreamerConfigurationAsync(string UniqueRequestId, CancellationToken cancellationToken = default);
        List<IClientConfiguration> GetClientStreamerConfigurations();
        Task<IChannelStatus?> GetOrCreateChannelStatusAsync(IClientConfiguration config, int streamGroupProfileId);
        IChannelStatus? GetChannelStatus(int smChannelId);
        List<IChannelStatus> GetChannelStatusFromStreamUrl(string videoUrl);
        IChannelStatus? GetChannelStatusFromSMChannelId(int smChannelId);
        List<IChannelStatus> GetChannelStatuses();
        List<IChannelStatus> GetChannelStatusesFromSMStreamId(string smStreamId);
        int GetGlobalStreamsCount();
        bool HasChannel(int SMChannelId);
        Task<IChannelStatus?> SetupChannelAsync(SMChannelDto smChannel);
        Task<bool> SwitchChannelToNextStreamAsync(IChannelStatus channelStatus, string? overrideNextVideoStreamId = null);
        Task<bool> UnRegisterClientAsync(string UniqueRequestId, CancellationToken cancellationToken = default);
    }
}
