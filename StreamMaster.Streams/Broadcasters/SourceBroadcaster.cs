using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;

using StreamMaster.Domain.Events;
using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Metrics;
using StreamMaster.Streams.Services;

namespace StreamMaster.Streams.Broadcasters;

/// <summary>
/// Represents a source broadcaster that manages the streaming of data to multiple channel broadcasters.
/// </summary>
public class SourceBroadcaster(ILogger<ISourceBroadcaster> logger, IOptionsMonitor<Setting> settings, IStreamFactory streamFactory, SMStreamInfo intSMStreamInfo)
    : ISourceBroadcaster, IDisposable
{
    private int _isStopped;

    public bool IsStopped => _isStopped == 1;
    private readonly StreamMetricsRecorder StreamMetricsRecorder = new();
    private readonly SemaphoreSlim _stopLock = new(1, 1);

    private Task? _streamingTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly TaskCompletionSource<bool> _broadcasterAddedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public SMStreamInfo SMStreamInfo { get; private set; } = intSMStreamInfo;

    /// <inheritdoc/>
    public StreamHandlerMetrics Metrics => StreamMetricsRecorder.Metrics;
    public bool IsMultiView { get; set; }

    /// <inheritdoc/>
    public ConcurrentDictionary<string, IStreamDataToClients> ChannelBroadcasters { get; } = new();

    /// <inheritdoc/>
    public event AsyncEventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStopped;

    /// <inheritdoc/>
    public bool IsFailed { get; }

    /// <inheritdoc/>
    public string Id => SMStreamInfo.Url;

    /// <inheritdoc/>
    public string Name => SMStreamInfo.Name;

    /// <inheritdoc/>
    public void AddChannelBroadcaster(IChannelBroadcaster channelBroadcaster)
    {
        AddChannelBroadcaster(channelBroadcaster.SMChannel.Id.ToString(), channelBroadcaster);
        _broadcasterAddedTcs.TrySetResult(true);
    }

    /// <inheritdoc/>
    public void AddChannelBroadcaster(string Id, IStreamDataToClients channelBroadcaster)
    {
        ChannelBroadcasters.TryAdd(Id, channelBroadcaster);
        _broadcasterAddedTcs.TrySetResult(true);
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
    public async Task<long> SetSourceMultiViewStreamAsync(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {StreamName}", Name, SMStreamInfo.Name);

        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            (Stream? stream, int processId, ProxyStreamError? error) =
                await streamFactory.GetMultiViewPlayListStream(channelBroadcaster, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            if (stream == null || error != null)
            {
                logger.LogError("Could not create source stream for channel broadcaster: {Id} {Name} {Error}", SMStreamInfo.Id, SMStreamInfo.Name, error?.Message);
                return 0;
            }

            // Start a new streaming task
            _cancellationTokenSource = new CancellationTokenSource();
            _streamingTask = Task.Run(() => RunPipelineAsync(stream, cancellationToken: _cancellationTokenSource.Token), _cancellationTokenSource.Token);
            return stopwatch.ElapsedMilliseconds;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <inheritdoc/>
    public async Task<long> SetSourceStreamAsync(SMStreamInfo SMStreamInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream {Name} to {StreamName}", Name, SMStreamInfo.Name);

        this.SMStreamInfo = SMStreamInfo;

        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            (Stream? stream, int processId, ProxyStreamError? error) =
                await streamFactory.GetStream(SMStreamInfo, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            if (stream == null || error != null)
            {
                logger.LogError("Could not create source stream for channel broadcaster: {Id} {Name} {Error}", SMStreamInfo.Id, SMStreamInfo.Name, error?.Message);
                return 0;
            }

            // Start a new streaming task
            _cancellationTokenSource = new CancellationTokenSource();
            _streamingTask = Task.Run(() => RunPipelineAsync(stream, cancellationToken: _cancellationTokenSource.Token), _cancellationTokenSource.Token);
            return stopwatch.ElapsedMilliseconds;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    /// <inheritdoc/>
    public async Task RunPipelineAsync(Stream sourceStream, int bufferSize = 8192, CancellationToken cancellationToken = default)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
        CancellationTokenSource? timeoutCts = null;
        CancellationTokenSource? linkedCts = null;
        try
        {
            if (ChannelBroadcasters.IsEmpty)
            {
                await _broadcasterAddedTcs.Task.ConfigureAwait(false);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                if (ChannelBroadcasters.IsEmpty)
                {
                    logger.LogWarning("No clients connected. Stopping the pipeline.");
                    break;
                }

                int bytesRead;

                try
                {
                    // Create timeout only if StreamReadTimeOutMs > 0
                    if (settings.CurrentValue.StreamReadTimeOutMs > 0)
                    {
                        timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(settings.CurrentValue.StreamReadTimeOutMs));
                    }

                    linkedCts = timeoutCts is not null
            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token)
            : CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                    bytesRead = await StreamMetricsRecorder.RecordMetricsAsync(
                        () => sourceStream.ReadAsync(buffer.AsMemory(0, bufferSize), linkedCts.Token),
                        linkedCts.Token);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    logger.LogWarning("Read operation was cancelled by global cancellation.");
                    break;
                }
                catch (OperationCanceledException) when (timeoutCts?.Token.IsCancellationRequested == true)
                {
                    logger.LogWarning("Read operation timed out after 500ms.");
                    break;
                }
                finally
                {
                    timeoutCts?.Dispose(); // Ensure timeoutCts is disposed
                }

                if (bytesRead == 0)
                {
                    logger.LogInformation("End of the source stream.");
                    break;
                }

                List<KeyValuePair<string, IStreamDataToClients>> broadcastersSnapshot = [.. ChannelBroadcasters];
                using SemaphoreSlim semaphore = new(10); // Limit concurrency to 10
                IEnumerable<Task> tasks = broadcastersSnapshot.Select(async broadcaster =>
                {
                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        await broadcaster.Value.StreamDataToClientsAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error streaming data to broadcaster {Id}.", broadcaster.Key);
                        ChannelBroadcasters.TryRemove(broadcaster.Key, out _);
                    }
                    finally
                    {
                        semaphore.Release();
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

            if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
            {
                logger.LogInformation("Source Broadcaster stopped: {Name}", Name);
                //if (cancellationToken.IsCancellationRequested)
                //{
                //    logger.LogInformation("Source Broadcaster stopped due to cancellation: {Name}", Name);
                //}
                //else
                if (OnStreamBroadcasterStopped != null)
                {
                    await OnStreamBroadcasterStopped.Invoke(this, new StreamBroadcasterStopped(Id, Name, cancellationToken.IsCancellationRequested));
                }
            }

            ChannelBroadcasters.Clear();
            await sourceStream.DisposeAsync().ConfigureAwait(false);
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