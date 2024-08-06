using StreamMaster.Domain.Models;
using StreamMaster.PlayList.Models;

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
public interface IChannelStatus : IStreamStatus, IChannelStatusBroadcaster
{
    //SMChannelDto SMChannel { get; }
    void SetIsGlobal();
    void SetSourceChannelBroadcaster(IChannelBroadcaster ChannelBroadcaster);

    //IChannelBroadcaster ChannelDistributor { get; set; }
    bool IsGlobal { get; set; }

}