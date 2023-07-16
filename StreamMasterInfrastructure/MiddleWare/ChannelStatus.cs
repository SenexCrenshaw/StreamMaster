using StreamMasterApplication.Common.Interfaces;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.MiddleWare;

public class ChannelStatus
{
    public  ConcurrentDictionary<Guid, Guid> ClientIds;

    public ChannelStatus(int videoStreamId, string videoStreamName)
    {
        VideoStreamId = videoStreamId;
        Rank = 0;
        ChannelWatcherToken = new CancellationTokenSource();
        FailoverInProgress = false;
        VideoStreamName = videoStreamName;
        ClientIds = new();
    }

    public CancellationTokenSource ChannelWatcherToken { get; set; }
    public bool FailoverInProgress { get; set; }

    public int Rank { get; set; }

    public IStreamInformation? StreamInformation { get; set; }
    public int VideoStreamId { get; set; }
    public string VideoStreamName { get; set; }

}
