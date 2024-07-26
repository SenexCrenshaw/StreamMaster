using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface IIntroStatus
{
    int IntroIndex { get; set; }
    bool PlayedIntro { get; set; }
    bool IsFirst { get; set; }
}

public abstract class IntroStatus : IIntroStatus
{
    public int IntroIndex { get; set; }
    public bool PlayedIntro { get; set; }
    public bool IsFirst { get; set; }
}
public interface IM3U8ChannelStatus : IIntroStatus
{
    void SetSMStreamInfo(IdNameUrl? idNameUrl);
    string ClientUserAgent { get; set; }
    CustomPlayList? CustomPlayList { get; set; }
    bool Shutdown { get; set; }
    string OverrideSMStreamId { get; set; }
    bool FailoverInProgress { get; set; }
    IdNameUrl? SMStreamInfo { get; }
    SMChannelDto SMChannel { get; set; }
    string M3U8File { get; }
    string M3U8Directory { get; }
}
