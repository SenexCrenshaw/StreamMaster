namespace StreamMaster.Streams.Channels;

public sealed class ChannelStatus(VideoStreamDto videoStreamDto) : IChannelStatus
{
    public bool IsStarted { get; set; }
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }

    public string OverrideVideoStreamId { get; set; } = string.Empty;
    public string ChannelVideoStreamId { get; set; } = videoStreamDto.Id;

    public string VideoStreamURL { get; set; } = videoStreamDto.User_Url;
    public string ChannelName { get; set; } = videoStreamDto.User_Tvg_name;
    public VideoStreamDto? CurrentVideoStream { get; private set; } = videoStreamDto;

    public void SetCurrentVideoStream(VideoStreamDto? CurrentVideoStream)
    {
        this.CurrentVideoStream = CurrentVideoStream;
    }

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}