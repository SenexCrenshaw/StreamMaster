using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.MiddleWare;

public class ChannelStatus
{

    public ChannelStatus(int videoStreamId, string videoStreamName)
    {
        VideoStreamId = videoStreamId;
        Rank = 0;
        ChannelWatcherToken = new CancellationTokenSource();
        FailoverInProgress = false;
        VideoStreamName = videoStreamName;

    }

    public CancellationTokenSource ChannelWatcherToken { get; set; }
    public bool FailoverInProgress { get; set; }

    public int Rank { get; set; }

    public IStreamInformation? StreamInformation { get; set; }
    public int VideoStreamId { get; set; }
    public string VideoStreamName { get; set; }

}
