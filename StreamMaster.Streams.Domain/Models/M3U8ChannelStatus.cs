using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Crypto;
using StreamMaster.PlayList.Models;

namespace StreamMaster.Streams.Domain.Models;

public sealed class M3U8ChannelStatus(SMChannelDto sMChannelDto) : IntroStatus, IM3U8ChannelStatus
{
    public bool Shutdown { get; set; } = false;
    public bool FailoverInProgress { get; set; }
    public SMChannelDto SMChannel { get; set; } = sMChannelDto;
    public string OverrideSMStreamId { get; set; } = string.Empty;
    public CustomPlayList? CustomPlayList { get; set; }
    public IdNameUrl? SMStreamInfo { get; private set; }
    public string ClientUserAgent { get; set; } = string.Empty;
    public string M3U8File { get; private set; }
    public string M3U8Directory { get; private set; }

    public void SetSMStreamInfo(IdNameUrl? idNameUrl)
    {
        SMStreamInfo = idNameUrl;
        if (idNameUrl == null)
        {
            M3U8Directory = string.Empty;
            M3U8File = string.Empty;
            return;
        }
        M3U8Directory = GetDirectory(idNameUrl.Id);
        M3U8File = Path.Combine(M3U8Directory, "index.m3u8");
    }

    public static string GetDirectory(string streamId)
    {
        return Path.Combine(BuildInfo.HLSOutputFolder, streamId.ToUrlSafeBase64String());
    }

}
