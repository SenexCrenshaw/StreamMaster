using StreamMaster.PlayList;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ChannelStatus(SMChannelDto smChannel) : IChannelStatus
{
    public bool Shutdown { get; set; } = false;
    public bool IsStarted { get; set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int CurrentRank { get; set; } = -1;
    public string OverrideVideoStreamId { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public SMStreamDto SMStream { get; private set; }
    public SMChannelDto SMChannel => smChannel;
    public VideoOutputProfileDto VideoProfile { get; set; }
    public CustomPlayList? CustomPlayList { get; set; }

    public void SetCurrentSMStream(SMStreamDto smStream)
    {
        SMStream = smStream;
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}