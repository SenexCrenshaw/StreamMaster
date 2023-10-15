using StreamMasterApplication.Common.Interfaces;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelStatus(string videoStreamId, string videoStreamName)
{
    public ConcurrentDictionary<Guid, Guid> ClientIds = new();
    public bool IsGlobal { get; set; }
    public CancellationTokenSource ChannelWatcherToken { get; set; } = new CancellationTokenSource();
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }
    public IStreamHandler? StreamController { get; set; }
    public string VideoStreamId { get; set; } = videoStreamId;
    public string VideoStreamName { get; set; } = videoStreamName;
}
