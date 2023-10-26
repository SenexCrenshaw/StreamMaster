using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public class ChannelStatus(string videoStreamId, string videoStreamName) : IChannelStatus
{
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }
    public string ChannelVideoStreamId { get; set; } = videoStreamId;
    public string CurrentVideoStreamId { get; set; } = videoStreamId;
    public string CurrentVideoStreamName { get; set; } = videoStreamName;
    public string VideoStreamURL { get; set; } = videoStreamName;

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}