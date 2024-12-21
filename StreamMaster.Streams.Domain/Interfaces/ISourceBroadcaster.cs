using System.Collections.Concurrent;

using StreamMaster.Domain.Models;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Statistics;

namespace StreamMaster.Streams.Domain.Interfaces;

public interface ISourceBroadcaster
{
    StreamHandlerMetrics? Metrics { get; }
    ConcurrentDictionary<string, IStreamDataToClients> ChannelBroadcasters { get; }
    void AddChannelBroadcaster(IChannelBroadcaster channelBroadcaster);
    void AddChannelBroadcaster(string Id, IStreamDataToClients channelBroadcaster);
    bool RemoveChannelBroadcaster(int ChannelBroadcasterId);
    Task StopAsync();
    bool IsFailed { get; }
    Task SetSourceStreamAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);
    string Id { get; }
    event EventHandler<StreamBroadcasterStopped> OnStreamBroadcasterStoppedEvent;
    public SMStreamInfo SMStreamInfo { get; }
}
