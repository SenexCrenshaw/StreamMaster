using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamStatus : IIntroStatus
{
    CustomPlayList? CustomPlayList { get; set; }
    bool Shutdown { get; set; }
    bool FailoverInProgress { get; set; }
    void SetSMStreamInfo(SMStreamInfo? idNameUrl);
    SMStreamInfo? SMStreamInfo { get; }
    SMChannelDto SMChannel { get; }
    int StreamGroupProfileId { get; set; }
    int StreamGroupId { get; set; }
}

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelStatus : IStreamStatus
{
    public int Id { get; }
    public string Name { get; }

    void SetIsGlobal();
    void SetSourceChannel(IChannelDistributor channelDistributor);

    void AddClient(string UniqueRequestId, IClientConfiguration config);
    bool RemoveClient(string UniqueRequestId);
    int ClientCount { get; }
    IChannelDistributor ChannelDistributor { get; set; }
    bool IsGlobal { get; set; }
    List<IClientConfiguration> GetClientStreamerConfigurations();
}