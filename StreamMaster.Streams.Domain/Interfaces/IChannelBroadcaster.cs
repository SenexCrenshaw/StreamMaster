using StreamMaster.Domain.Models;
using StreamMaster.PlayList.Models;
using StreamMaster.Streams.Domain.Events;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ISourceName
{
    string SourceName { get; }
}

public interface IStreamStatus : IIntroStatus, ISourceName
{
    SMChannelDto SMChannel { get; }
    CustomPlayList? CustomPlayList { get; set; }
    bool Shutdown { get; set; }
    bool FailoverInProgress { get; set; }
    void SetSMStreamInfo(SMStreamInfo? idNameUrl);
    SMStreamInfo? SMStreamInfo { get; }

    int StreamGroupProfileId { get; set; }
    //int StreamGroupId { get; set; }
}

/// <summary>
/// Provides methods and properties to manage the status and configuration of a channel.
/// </summary>
public interface IChannelBroadcaster : IBroadcasterBase, IStreamStatus
{
    /// <summary>
    /// Gets the unique identifier for the channel.
    /// </summary>
    int Id { get; }
    /// <summary>
    /// Occurs when the channel director is stopped.
    /// </summary>
    event EventHandler<ChannelBroascasterStopped>? OnChannelStatusStoppedEvent;

    //SMChannelDto SMChannel { get; }
    void SetIsGlobal();
    void SetSourceChannelBroadcaster(IStreamBroadcaster ChannelBroadcaster);

    //IStreamBroadcaster ChannelDistributor { get; set; }
    bool IsGlobal { get; set; }

}