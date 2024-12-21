using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;

using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Statistics;
using StreamMaster.Streams.Services;
namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster(ILogger<ISourceBroadcaster> logger, IStreamFactory streamFactory, SMStreamInfo smStreamInfo)
    : ISourceBroadcaster
{
    private int _isStopped;
    private readonly StreamMetricsTracker MetricsService = new();
    public ConcurrentDictionary<string, IStreamDataToClients> ChannelBroadcasters { get; } = new();

    private Task? _streamingTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly SemaphoreSlim _stopLock = new(1, 1);

    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;
    public SMStreamInfo SMStreamInfo => smStreamInfo;
    private readonly TaskCompletionSource<bool> _broadcasterAddedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void AddChannelBroadcaster(IChannelBroadcaster channelBroadcaster)
    {
        AddChannelBroadcaster(channelBroadcaster.SMChannel.Id.ToString(), channelBroadcaster);
        _broadcasterAddedTcs.TrySetResult(true);
    }

    public void AddChannelBroadcaster(string Id, IStreamDataToClients channelBroadcaster)
    {
        ChannelBroadcasters.TryAdd(Id, channelBroadcaster);
    }

    public string StringId()
    {
        return Id;
    }
    public bool IsFailed { get; set; } = false;

    public async Task SetSourceStreamAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {streamName}", Name, smStreamInfo.Name);
        (Stream? stream, int? processId, ProxyStreamError? error) = await streamFactory.GetStream(channelBroadcaster, cancellationToken).ConfigureAwait(false);
        if (stream == null || processId == null || error != null)
        {
            logger.LogError("Could not create source stream for channel distributor: {Id} {name} {error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
            return;
        }
        // await StopAsync().ConfigureAwait(false);

        // Start a new task for streaming
        _cancellationTokenSource = new CancellationTokenSource();
        _streamingTask = Task.Run(() => RunPipelineAsync(stream, cancellationToken: _cancellationTokenSource.Token), _cancellationTokenSource.Token);
    }

    public async Task RunPipelineAsync(Stream sourceStream, int bufferSize = 8192, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = new();
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            // Wait for the first broadcaster to be added
            if (ChannelBroadcasters.IsEmpty)
            {
                logger.LogInformation("Waiting for the first broadcaster to be added...");
                await _broadcasterAddedTcs.Task.ConfigureAwait(false);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                if (ChannelBroadcasters.IsEmpty)
                {
                    logger.LogWarning("No clients connected. Stopping the pipeline.");
                    break;
                }

                stopwatch.Restart();
                int bytesRead = await sourceStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                stopwatch.Stop();

                if (bytesRead == 0)
                {
                    logger.LogInformation("End of the source stream.");
                    break;
                }

                double latency = stopwatch.Elapsed.TotalMilliseconds;
                MetricsService.RecordMetrics(bytesRead, latency);

                IEnumerable<Task> tasks = ChannelBroadcasters.Select(async channel =>
                {
                    try
                    {
                        await channel.Value.StreamDataToClientsAsync(new ReadOnlySequence<byte>(buffer, 0, bytesRead), cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error occurred while streaming data to clients.");
                        ChannelBroadcasters.TryRemove(channel.Key, out _);
                    }
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error occurred during pipeline streaming");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);

            ChannelBroadcasters.Clear();
            await sourceStream.DisposeAsync().ConfigureAwait(false);

            stopwatch.Stop();

            if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
            {
                logger.LogInformation("Source Broadcaster stopped: {Name}", Name);
                OnStreamBroadcasterStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
            }
        }
    }


    public StreamHandlerMetrics Metrics => MetricsService.Metrics;

    public async Task StopAsync()
    {
        await _stopLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_isStopped == 1)
            {
                return;
            }

            //foreach (KeyValuePair<string, PipeWriter> channelBroadcaster in ChannelBroadcasters)
            //{
            //    channelBroadcaster.Value.Complete();
            //}
            //ChannelBroadcasters.Clear();

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();

                if (_streamingTask != null)
                {
                    try
                    {
                        await _streamingTask.ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        // Expected when the task is canceled
                    }
                }

                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                _streamingTask = null;

                logger.LogInformation("Streaming task stopped.");
            }
        }
        finally
        {
            //if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
            //{
            //    // Derived-specific logic before stopping
            //    logger.LogInformation("Source Broadcaster stopped: {Name}", Name);

            //    // Additional cleanup or finalization
            //    OnStreamBroadcasterStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
            //}

            _stopLock.Release();
        }
    }

    public bool RemoveChannelBroadcaster(int ChannelBroadcasterId)
    {
        return ChannelBroadcasters.TryRemove(ChannelBroadcasterId.ToString(), out _);
    }

    /// <inheritdoc/>
    public string Id => smStreamInfo.Url;

    public string Name => smStreamInfo.Name;
}
