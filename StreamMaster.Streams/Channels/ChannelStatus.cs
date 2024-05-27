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
    public SMStreamDto CurrentSMStream { get; private set; }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}