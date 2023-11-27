using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

namespace StreamMasterInfrastructure.VideoStreamManager.Channels;

public class ChannelStatus(string videoStreamId, string videoStreamName, string channelName) : IChannelStatus
{
    public bool IsGlobal { get; set; }
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }
    public string ChannelVideoStreamId { get; set; } = videoStreamId;

    public string VideoStreamURL { get; set; } = videoStreamName;
    public string ChannelName { get; set; } = channelName;
    public VideoStreamDto CurrentVideoStream { get; set; } = new();

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
}