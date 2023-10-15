using StreamMasterApplication.Common.Interfaces;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelStatus(string videoStreamId, string videoStreamName) : IChannelStatus
{
    public ConcurrentDictionary<Guid, Guid> ClientIds = new();
    public bool IsGlobal { get; set; }
    public CancellationTokenSource ChannelWatcherToken { get; set; } = new CancellationTokenSource();
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }
    public IStreamHandler? StreamHandler { get; set; }
    public string VideoStreamId { get; set; } = videoStreamId;
    public string VideoStreamName { get; set; } = videoStreamName;
    public List<Guid> GetChannelClientIds => ClientIds.Values.ToList();

    List<Guid> IChannelStatus.GetChannelClientIds => ClientIds.Values.ToList();

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
    public void AddToClientIds(Guid clientId)
    {
        ClientIds.TryAdd(clientId, clientId);
    }
    public void RemoveClientId(Guid clientId)
    {
        ClientIds.TryRemove(clientId, out _);
    }
}
