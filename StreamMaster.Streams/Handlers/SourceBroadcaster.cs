using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Pipelines;

using StreamMaster.Streams.Domain.Events;
using StreamMaster.Streams.Domain.Statistics;
using StreamMaster.Streams.Services;
namespace StreamMaster.Streams.Handlers;

public class SourceBroadcaster(ILogger<ISourceBroadcaster> logger, IStreamFactory streamFactory, SMStreamInfo smStreamInfo, IOptionsMonitor<Setting> _settings)
    : ISourceBroadcaster
{
    private int _isStopped;
    private readonly StreamMetricsTracker MetricsService = new();
    public ConcurrentDictionary<string, PipeWriter> ChannelBroadcasters { get; } = new();

    private Task? _streamingTask;
    private CancellationTokenSource? _cancellationTokenSource;
    private readonly SemaphoreSlim _stopLock = new(1, 1);

    public event EventHandler<StreamBroadcasterStopped>? OnStreamBroadcasterStoppedEvent;
    public SMStreamInfo SMStreamInfo => smStreamInfo;
    private readonly TaskCompletionSource<bool> _broadcasterAddedTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void AddChannelBroadcaster(IChannelBroadcaster channelBroadcaster)
    {
        AddChannelBroadcaster(channelBroadcaster.SMChannel.Id.ToString(), channelBroadcaster.Pipe.Writer);
        _broadcasterAddedTcs.TrySetResult(true);
    }

    public void AddChannelBroadcaster(string Id, PipeWriter pipeWriter)
    {
        //if (ChannelBroadcasters.TryAdd(Id, pipeWriter))
        //{
        //    // Signal that a broadcaster has been added
        //    _broadcasterAddedTcs.TrySetResult(true);
        //}
        ChannelBroadcasters.TryAdd(Id, pipeWriter);
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

        try
        {
            // Wait for the first broadcaster to be added
            if (ChannelBroadcasters.IsEmpty)
            {
                logger.LogInformation("Waiting for the first broadcaster to be added...");
                await _broadcasterAddedTcs.Task.ConfigureAwait(false);
            }

            // Read from the source stream and write directly to all clients
            while (!cancellationToken.IsCancellationRequested)
            {
                if (ChannelBroadcasters.IsEmpty)
                {
                    logger.LogWarning("No clients connected. Stopping the pipeline.");
                    break; // Stop if no clients are connected
                }

                byte[] buffer = new byte[bufferSize];
                stopwatch.Restart();
                int bytesRead = await sourceStream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
                stopwatch.Stop();

                if (bytesRead == 0)
                {
                    logger.LogInformation("End of the source stream.");
                    break; // End of the stream
                }

                double latency = stopwatch.Elapsed.TotalMilliseconds;
                MetricsService.RecordMetrics(bytesRead, latency);

                // Write data to all clients
                foreach (KeyValuePair<string, PipeWriter> kvp in ChannelBroadcasters.ToArray()) // Safe copy for iteration
                {
                    if (_isStopped == 1 || cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    string key = kvp.Key;
                    PipeWriter writer = kvp.Value;

                    try
                    {
                        writer.Write(buffer.AsSpan(0, bytesRead)); // Write only the valid portion of the buffer
                        await writer.FlushAsync(cancellationToken).ConfigureAwait(false); // Flush immediately
                    }
                    catch (InvalidOperationException)
                    {
                        logger.LogWarning("PipeWriter for {Key} is completed. Removing from broadcasters.", key);
                        ChannelBroadcasters.TryRemove(key, out _); // Remove completed writer
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to write to client {ClientId}. Removing client.", key);
                        ChannelBroadcasters.TryRemove(key, out _); // Remove problematic client
                    }
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error occurred during pipeline streaming");
        }
        finally
        {
            // Complete all writers
            foreach (PipeWriter writer in ChannelBroadcasters.Values)
            {
                await writer.CompleteAsync().ConfigureAwait(false);
            }
            ChannelBroadcasters.Clear();
            // Dispose of the source stream
            await sourceStream.DisposeAsync().ConfigureAwait(false);

            stopwatch.Stop();
            // Stop the broadcaster
            //await StopAsync();
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

            if (Interlocked.CompareExchange(ref _isStopped, 1, 0) == 0)
            {
                // Derived-specific logic before stopping
                logger.LogInformation("Source Broadcaster stopped: {Name}", Name);


                // Additional cleanup or finalization
                OnStreamBroadcasterStoppedEvent?.Invoke(this, new StreamBroadcasterStopped(Id, Name));
            }

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
