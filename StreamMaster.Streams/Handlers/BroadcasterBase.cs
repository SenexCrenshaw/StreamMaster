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
    public ConcurrentDictionary<string, ChannelWriter<byte[]>> ClientChannelWriters { get; } = new();

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

        if (!ClientChannelWriters.IsEmpty)
        {
            foreach (KeyValuePair<string, ChannelWriter<byte[]>> client in ClientChannelWriters)
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

    public void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader, string sourceChannelName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source channel for {Name} to {sourceChannelName}", Name, sourceChannelName);
        SourceName = sourceChannelName;
        Channel<byte[]> newChannel = ChannelHelper.GetChannel();
        StartProcessingSource(sourceChannelReader, null, newChannel, cancellationToken);
    }

    public void SetSourceStream(Stream sourceStream, string streamName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {streamName}", Name, streamName);
        SourceName = streamName;
        Channel<byte[]> newChannel = ChannelHelper.GetChannel();

        StartProcessingSource(null, sourceStream, newChannel, cancellationToken);
    }

    protected void StartProcessingSource(ChannelReader<byte[]>? sourceChannelReader, Stream? sourceStream, Channel<byte[]> newChannel, CancellationToken cancellationToken)
    {
        //using CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        //CancellationToken token = linkedTokenSource.Token;

        Task readerTask = sourceChannelReader != null
       ? sourceProcessingService.ProcessSourceChannelReaderAsync(sourceChannelReader, newChannel, metricsService, _cancellationTokenSource.Token)
       : sourceStream != null
           ? sourceProcessingService.ProcessSourceStreamAsync(sourceStream, newChannel, metricsService, _cancellationTokenSource.Token)
           : Task.CompletedTask;

        Task writerTask = DistributeToClientsAsync(newChannel, _cancellationTokenSource.Token);
        _ = MonitorTasksCompletionAsync(readerTask, writerTask, sourceStream, _cancellationTokenSource.Token);
    }

    private Task DistributeToClientsAsync(Channel<byte[]> newChannel, CancellationToken token)
    {
        List<Task> writeTasks = [];

        return Task.Run(async () =>
        {
            try
            {
                await foreach (byte[] item in newChannel.Reader.ReadAllAsync(token))
                {
                    writeTasks.Clear();
                    foreach (ChannelWriter<byte[]> clientChannel in ClientChannelWriters.Values)
                    {
                        writeTasks.Add(clientChannel.WriteAsync(item, token).AsTask());
                    }
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

    public void AddChannelStreamer(int smChannelId, ChannelWriter<byte[]> channel)
    {
        AddChannelStreamer(smChannelId.ToString(), channel);
    }

    public bool RemoveChannelStreamer(int smChannelId)
    {
        return RemoveChannelStreamer(smChannelId.ToString());
    }

    public void AddChannelStreamer(string uniqueRequestId, ChannelWriter<byte[]> channel)
    {
        logger.LogInformation("Add channel streamer: {UniqueRequestId} to {Name}", uniqueRequestId, Name);
        ClientChannelWriters.TryAdd(uniqueRequestId, channel);
    }

    public bool RemoveChannelStreamer(string uniqueRequestId)
    {
        if (ClientChannelWriters.TryRemove(uniqueRequestId, out _))
        {
            logger.LogInformation("Remove channel streamer: {UniqueRequestId} {Name}", uniqueRequestId, Name);
            return true;
        }
        return false;
    }

    public void AddClientStreamer(string uniqueRequestId, IClientConfiguration config)
    {
        if (ClientStreamerConfigurations.TryAdd(uniqueRequestId, config))
        {
            logger.LogInformation("Add client streamer: {UniqueRequestId} to {Name}", uniqueRequestId, Name);
            ClientChannelWriters.TryAdd(uniqueRequestId, config.ClientStream!.Channel);
        }
    }

    public bool RemoveClientStreamer(string uniqueRequestId)
    {
        if (ClientStreamerConfigurations.TryRemove(uniqueRequestId, out IClientConfiguration? clientConfiguration) && ClientChannelWriters.TryRemove(uniqueRequestId, out _))
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