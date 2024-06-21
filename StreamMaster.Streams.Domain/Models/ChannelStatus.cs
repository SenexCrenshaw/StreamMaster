using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Models;

public sealed class ChannelStatus(SMChannel smChannel) : IChannelStatus
{
    public bool IsStarted { get; set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }
    public string OverrideVideoStreamId { get; set; } = string.Empty;
    public int ClientCount { get; set; }
    public SMStream SMStream { get; private set; }
    public SMChannel SMChannel => smChannel;
    public VideoOutputProfileDto VideoProfile { get; set; }

    public void SetCurrentSMStream(SMStream smStream)
    {
        SMStream = smStream;
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}