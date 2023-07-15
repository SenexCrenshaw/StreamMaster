using StreamMasterApplication.Common.Interfaces;

namespace StreamMasterInfrastructure.MiddleWare;

public class ChannelStatus
{
    //private readonly ConcurrentDictionary<Guid, ClientStreamerConfiguration> _clientStreamerConfigurations;

    public ChannelStatus(int VideoStreamId)
    {
        this.VideoStreamId = VideoStreamId;
        Rank = 0;
        ChannelWatcherToken = new CancellationTokenSource();
        FailoverInProgress = false;
        //_clientStreamerConfigurations = new();
    }

    public CancellationTokenSource ChannelWatcherToken { get; set; }
    public bool FailoverInProgress { get; set; }

    public int Rank { get; set; }

    public IStreamInformation? StreamInformation { get; set; }
    public int VideoStreamId { get; set; }

    //public void AddClientStreamerConfiguration(ClientStreamerConfiguration clientStreamerConfiguration)
    //{
    //    _clientStreamerConfigurations.TryAdd(clientStreamerConfiguration.ClientId, clientStreamerConfiguration);
    //}

    //public int GetClientStreamerCount()
    //{
    //    return _clientStreamerConfigurations.Count;
    //}

    //public void RemoveClientStreamerConfiguration(Guid ClientId)
    //{
    //    _clientStreamerConfigurations.TryRemove(ClientId, out _);
    //}
}
