using StreamMaster.Domain.Configuration;
using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamStatus : IIntroStatus
{
    string? OverrideSMStreamId { get; set; }
    CustomPlayList? CustomPlayList { get; set; }
    bool Shutdown { get; set; }
    string ClientUserAgent { get; set; }
    bool FailoverInProgress { get; set; }
    void SetSMStreamInfo(SMStreamInfo? idNameUrl);
    SMStreamInfo? SMStreamInfo { get; }
    SMChannelDto SMChannel { get; }
}

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelStatus : IStreamStatus
{
    int StreamGroupProfileId { get; set; }
    CommandProfileDto CommandProfile { get; set; }
    void SetIsGlobal();
    void SetSourceChannel(IChannelDistributor channelDistributor, string Name, bool IsCustom);

    void AddClient(string UniqueRequestId, IClientConfiguration config);
    bool RemoveClient(string UniqueRequestId);
    int ClientCount { get; }
    IChannelDistributor ChannelDistributor { get; }
    bool IsGlobal { get; set; }
    List<IClientConfiguration> GetClientStreamerConfigurations();
}