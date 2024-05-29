namespace StreamMaster.Streams.Channels;

public sealed class ChannelStatus(SMChannel smChannel) : IChannelStatus
{
    public bool IsStarted { get; set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }

    public string OverrideVideoStreamId { get; set; } = string.Empty;
    public int Id { get; set; } = smChannel.Id;

    public string ChannelName { get; set; } = smChannel.Name;
    public SMStream SMStream { get; private set; }
    public SMChannel SMChannel => smChannel;

    public void SetCurrentSMStream(SMStream smStream)
    {
        SMStream = smStream;
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}