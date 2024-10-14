
using StreamMaster.Streams.Domain;
using StreamMaster.Streams.Domain.Helpers;
using StreamMaster.Streams.Domain.Statistics;
using StreamMaster.Streams.Services;

using System.Collections.Concurrent;
using System.Threading.Channels;
using System.Xml.Serialization;

namespace StreamMaster.Streams.Handlers;

public abstract class BroadcasterBase(ILogger<IBroadcasterBase> logger, IOptionsMonitor<Setting> _settings) : IBroadcasterBase
{
    private readonly MetricsService metricsService = new();
    private readonly SourceProcessingService sourceProcessingService = new(logger, _settings);
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private long _channelItemCount;

    [XmlIgnore]
    public ConcurrentDictionary<string, IClientConfiguration> ClientStreamerConfigurations { get; set; } = new();

    [XmlIgnore]
    public ConcurrentDictionary<string, TrackedChannel> ClientChannels { get; } = new();

    public int ClientCount => ClientStreamerConfigurations.Keys.Count;
    public bool IsFailed { get; set; } = false;
    public virtual string Name { get; set; } = string.Empty;

    [XmlIgnore]
    public string SourceName { get; private set; } = string.Empty;
    public long ChannelItemBackLog => Interlocked.Read(ref _channelItemCount);

    public virtual void Stop()
    {
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            logger.LogInformation("Stopped broadcaster for: {Name}", Name);
            _cancellationTokenSource.Cancel();
        }

        if (!ClientChannels.IsEmpty)
        {
            foreach (KeyValuePair<string, TrackedChannel> client in ClientChannels)
            {
                RemoveChannelStreamer(client.Key);
            }
        }
    }

    public virtual string StringId()
    {
        return "NA";
    }

    public StreamHandlerMetrics Metrics => metricsService.Metrics;
    public TrackedChannel Channel { get; } = ChannelHelper.GetChannel();

    public void SetSourceChannel(TrackedChannel sourceChannelReader, string sourceChannelName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source channel for {Name} to {sourceChannelName}", Name, sourceChannelName);
        SourceName = sourceChannelName;
        StartProcessingSource(sourceChannelReader, null, Channel, cancellationToken);
    }

    public void SetSourceStream(Stream sourceStream, string streamName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {streamName}", Name, streamName);
        SourceName = streamName;
        StartProcessingSource(null, sourceStream, Channel, cancellationToken);
    }

    protected void StartProcessingSource(TrackedChannel? sourceChannelReader, Stream? sourceStream, TrackedChannel newChannel, CancellationToken cancellationToken)
    {

        Task readerTask = sourceChannelReader != null
       ? sourceProcessingService.ProcessSourceChannelReaderAsync(sourceChannelReader, newChannel, metricsService, _cancellationTokenSource.Token)
       : sourceStream != null
           ? sourceProcessingService.ProcessSourceStreamAsync(sourceStream, newChannel, metricsService, _cancellationTokenSource.Token)
           : Task.CompletedTask;

        Task writerTask = DistributeToClientsAsync(newChannel, _cancellationTokenSource.Token);
        _ = MonitorTasksCompletionAsync(readerTask, writerTask, sourceStream, _cancellationTokenSource.Token);
    }

    private Task DistributeToClientsAsync(TrackedChannel newChannel, CancellationToken token)
    {
        List<Task> writeTasks = [];

        return Task.Run(async () =>
        {
            try
            {
                await foreach (byte[] item in newChannel.ReadAllAsync(token))
                {
                    writeTasks.Clear();

                    // Use the same item (byte[]) to distribute to clients without creating new arrays
                    foreach (TrackedChannel clientChannel in ClientChannels.Values)
                    {
                        writeTasks.Add(clientChannel.WriteAsync(item, token).AsTask());
                    }

                    // Await the tasks and return buffers to the pool if necessary
                    await Task.WhenAll(writeTasks).ConfigureAwait(false);
                    Interlocked.Decrement(ref _channelItemCount);
                }
            }
            catch (OperationCanceledException ex)
            {
                logger.LogInformation("Stream operation stopped for: {name}", Name);
                logger.LogDebug(ex, "Stream operation stopped for: {name}", Name);
            }
            catch (ChannelClosedException ex)
            {
                logger.LogInformation("Stream operation stopped for: {name}", Name);
                logger.LogDebug(ex, "Stream channel closed for: {name}", Name);
            }
            catch (Exception ex)
            {
                logger.LogInformation("Stream operation stopped for: {name}", Name);
                logger.LogError(ex, "Unexpected error during stream reading for: {name}", Name);
            }
        }, token);
    }

    private Task MonitorTasksCompletionAsync(Task readerTask, Task writerTask, Stream? sourceStream, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                await Task.WhenAny(readerTask, writerTask).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
            }
            finally
            {
                IsFailed = true;
                if (sourceStream is IAsyncDisposable asyncDisposableStream)
                {
                    await asyncDisposableStream.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    sourceStream?.Dispose();
                }
                Stop();
            }
        }, token);
    }

    public bool IsChannelEmpty()
    {
        return ChannelItemBackLog == 0;
    }

    public void AddChannelStreamer(int smChannelId, TrackedChannel channel)
    {
        AddChannelStreamer(smChannelId.ToString(), channel);
    }

    public bool RemoveChannelStreamer(int smChannelId)
    {
        return RemoveChannelStreamer(smChannelId.ToString());
    }

    public void AddChannelStreamer(string uniqueRequestId, TrackedChannel channel)
    {
        logger.LogInformation("Add channel streamer: {UniqueRequestId} to {Name}", uniqueRequestId, Name);
        ClientChannels.TryAdd(uniqueRequestId, channel);
    }

    public bool RemoveChannelStreamer(string uniqueRequestId)
    {
        if (ClientChannels.TryRemove(uniqueRequestId, out TrackedChannel? trackedChannel))
        {
            trackedChannel.Dispose(); // Ensure proper disposal of the channel
            logger.LogInformation("Removed channel streamer: {UniqueRequestId} {Name}", uniqueRequestId, Name);
            return true;
        }
        return false;
    }

    public void AddClientStreamer(string uniqueRequestId, IClientConfiguration config)
    {
        if (ClientStreamerConfigurations.TryAdd(uniqueRequestId, config))
        {
            logger.LogInformation("Add client streamer: {UniqueRequestId} to {Name}", uniqueRequestId, Name);
            ClientChannels.TryAdd(uniqueRequestId, config.ClientStream!.Channel);
        }
    }

    public bool RemoveClientStreamer(string uniqueRequestId)
    {
        if (ClientStreamerConfigurations.TryRemove(uniqueRequestId, out IClientConfiguration? clientConfiguration) && ClientChannels.TryRemove(uniqueRequestId, out _))
        {
            logger.LogInformation("Removed client streamer: {UniqueRequestId} {Name}", uniqueRequestId, Name);
            clientConfiguration.Stop();
            return true;
        }
        return false;
    }

    public List<IClientConfiguration> GetClientStreamerConfigurations()
    {
        return [.. ClientStreamerConfigurations.Values];
    }
}
