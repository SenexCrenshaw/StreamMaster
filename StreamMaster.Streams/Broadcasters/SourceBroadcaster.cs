using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;

using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Statistics;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Broadcasters;

/// <summary>
/// Represents a source broadcaster that manages the streaming of data to multiple channel broadcasters.
/// </summary>
public class SourceBroadcaster(ILogger<ISourceBroadcaster> logger, IStreamFactory streamFactory, SMStreamInfo smStreamInfo)
    : ISourceBroadcaster, IDisposable
{
    private int _isStopped;
    private readonly StreamMetricsRecorder StreamMetricsRecorder = new();
    private readonly SemaphoreSlim _stopLock = new(1, 1);

    private Task? _streamingTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly TaskCompletionSource<bool> _broadcasterAddedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <inheritdoc/>
    public StreamHandlerMetrics Metrics => StreamMetricsRecorder.Metrics;

    /// <inheritdoc/>
    public ConcurrentDictionary<string, IStreamDataToClients> ChannelBroadcasters { get; } = new();

    /// <inheritdoc/>
    public event EventHandler<StreamBroadcasterStopped>? StreamBroadcasterStopped;

    /// <inheritdoc/>
    public bool IsFailed { get; }

    /// <inheritdoc/>
    public string Id => smStreamInfo.Url;

    /// <inheritdoc/>
    public string Name => smStreamInfo.Name;

    /// <inheritdoc/>
    public void AddChannelBroadcaster(IChannelBroadcaster channelBroadcaster)
    {
        AddChannelBroadcaster(channelBroadcaster.SMChannel.Id.ToString(), channelBroadcaster);
        _broadcasterAddedTcs.TrySetResult(true);
    }

    /// <inheritdoc/>
    public void AddChannelBroadcaster(string id, IStreamDataToClients channelBroadcaster)
    {
        ChannelBroadcasters.TryAdd(id, channelBroadcaster);
    }

    /// <inheritdoc/>
    public bool RemoveChannelBroadcaster(int channelBroadcasterId)
    {
        return RemoveChannelBroadcaster(channelBroadcasterId.ToString());
    }

    /// <inheritdoc/>
    public bool RemoveChannelBroadcaster(string Id)
    {
        return ChannelBroadcasters.TryRemove(Id, out _);
    }
    public async Task SetSourceMultiViewStreamAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {StreamName}", Name, smStreamInfo.Name);

        (Stream? stream, int processId, ProxyStreamError? error) =
            await streamFactory.GetMultiViewPlayListStream(channelBroadcaster, cancellationToken).ConfigureAwait(false);

        if (stream == null || error != null)
        {
            logger.LogError("Could not create source stream for channel broadcaster: {Id} {Name} {Error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
            return;
        }

        // Start a new streaming task
        _cancellationTokenSource = new CancellationTokenSource();
        _streamingTask = Task.Run(() => RunPipelineAsync(stream, cancellationToken: _cancellationTokenSource.Token), _cancellationTokenSource.Token);
    }

    /// <inheritdoc/>
    public async Task SetSourceStreamAsync(SMStreamInfo smStreamInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {StreamName}", Name, smStreamInfo.Name);

        (Stream? stream, int processId, ProxyStreamError? error) =
            await streamFactory.GetStream(smStreamInfo, cancellationToken).ConfigureAwait(false);

        if (stream == null || error != null)
        {
            logger.LogError("Could not create source stream for channel broadcaster: {Id} {Name} {Error}", smStreamInfo.Id, smStreamInfo.Name, error?.Message);
            return;
        }

        // Start a new streaming task
        _cancellationTokenSource = new CancellationTokenSource();
        _streamingTask = Task.Run(() => RunPipelineAsync(stream, cancellationToken: _cancellationTokenSource.Token), _cancellationTokenSource.Token);
    }

    /// <inheritdoc/>
    public async Task RunPipelineAsync(Stream sourceStream, int bufferSize = 8192, CancellationToken cancellationToken = default)
    {
        Stopwatch stopwatch = new();
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            if (ChannelBroadcasters.IsEmpty)
            {
                //logger.LogInformation("Waiting for the first broadcaster to be added...");
                await _broadcasterAddedTcs.Task.ConfigureAwait(false);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                if (ChannelBroadcasters.IsEmpty)
                {
                    logger.LogWarning("No clients connected. Stopping the pipeline.");
                    break;
                }

                //stopwatch.Restart();
                //int bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken).ConfigureAwait(false);
                //stopwatch.Stop();

                int bytesRead = await StreamMetricsRecorder.RecordMetricsAsync(
                            () => sourceStream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken),
                            cancellationToken);

                if (bytesRead == 0)
                {
                    logger.LogInformation("End of the source stream.");
                    break;
                }

                //StreamMetricsRecorder.RecordMetrics(bytesRead, stopwatch.Elapsed.TotalMilliseconds);

                IEnumerable<Task> tasks = ChannelBroadcasters.Select(async broadcaster =>
                {
                    try
                    {
                        await broadcaster.Value.StreamDataToClientsAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error streaming data to broadcaster {Id}.", broadcaster.Key);
                        ChannelBroadcasters.TryRemove(broadcaster.Key, out _);
                    }
                });

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unexpected error occurred during pipeline streaming.");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
            ChannelBroadcasters.Clear();
            await sourceStream.DisposeAsync().ConfigureAwait(false);

            if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
            {
                logger.LogInformation("Source Broadcaster stopped: {Name}", Name);
                StreamBroadcasterStopped?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
            }
        }
    }

    /// <inheritdoc/>
    public async Task StopAsync()
    {
        await _stopLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (_isStopped == 1)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();

            if (_streamingTask != null)
            {
                try
                {
                    await _streamingTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected during cancellation
                }
            }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _streamingTask = null;

            logger.LogInformation("Streaming task stopped.");
        }
        finally
        {
            _stopLock.Release();
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        StopAsync().GetAwaiter().GetResult();
        _stopLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
