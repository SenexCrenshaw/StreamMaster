using StreamMaster.Domain.Models;
using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamStatus : IIntroStatus, ISourceName
{
    SMChannelDto SMChannel { get; }
    CustomPlayList? CustomPlayList { get; set; }
    bool Shutdown { get; set; }
    bool FailoverInProgress { get; set; }
    void SetSMStreamInfo(SMStreamInfo? idNameUrl);
    SMStreamInfo? SMStreamInfo { get; }
    int StreamGroupProfileId { get; }
}
