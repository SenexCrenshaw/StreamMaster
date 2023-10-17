using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class ChannelStatus(string videoStreamId, string videoStreamName) : IChannelStatus
{
    public ConcurrentDictionary<Guid, ClientStreamerConfiguration> ClientIds = new();
    public bool IsGlobal { get; set; }
    public CancellationTokenSource ChannelWatcherToken { get; set; } = new CancellationTokenSource();
    public bool FailoverInProgress { get; set; }
    public int Rank { get; set; }
    public string ParentVideoStreamId { get; set; } = videoStreamId;
    public string VideoStreamId { get; set; } = videoStreamId;
    public string VideoStreamName { get; set; } = videoStreamName;
    public string VideoStreamURL { get; set; } = videoStreamName;
    public List<ClientStreamerConfiguration> GetChannelClientClientStreamerConfigurations => ClientIds.Values.ToList();

    public int ClientCount => ClientIds.Count;

    public void SetIsGlobal()
    {
        IsGlobal = true;
    }
    public void RegisterClient(ClientStreamerConfiguration clientStreamerConfiguration)
    {
        ClientIds.TryAdd(clientStreamerConfiguration.ClientId, clientStreamerConfiguration);
    }
    public void UnRegisterClient(Guid clientId)
    {
        ClientIds.TryRemove(clientId, out _);
    }
}
