
using StreamMaster.Streams.Domain.Helpers;
using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;
using System.Xml.Serialization;

namespace StreamMaster.Streams.Handlers;

public class BroadcasterBase : IBroadcasterBase
{
    public BroadcasterBase() { ClientStreamerConfigurations = new(); }

    /// <inheritdoc/> 
    [XmlIgnore]
    public ConcurrentDictionary<string, IClientConfiguration> ClientStreamerConfigurations { get; set; } = new();

    /// <inheritdoc/>
    [XmlIgnore]
    public ConcurrentDictionary<string, ChannelWriter<byte[]>> ClientChannelWriters { get; } = new();


    private readonly Meter _meter;
    private readonly Counter<long> _bytesReadCounter;
    private readonly Histogram<double> _kbpsHistogram;
    private readonly Histogram<double> _latencyHistogram;
    private readonly BPSStatistics _bpsStatistics = new();
    private long _bytesRead;
    private long _channelItemCount;
    private readonly DateTime _startTime = DateTime.UtcNow;
    private readonly ConcurrentQueue<double> _latencies = new();
    internal readonly ILogger<IBroadcasterBase> logger;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public BroadcasterBase(ILogger<IBroadcasterBase> logger)
    {
        this.logger = logger;
        _meter = new Meter("StreamHandlerMetrics", "1.0");
        _bytesReadCounter = _meter.CreateCounter<long>("bytes_read");
        _kbpsHistogram = _meter.CreateHistogram<double>("kbps", "kbps");
        _latencyHistogram = _meter.CreateHistogram<double>("latency", "ms");
    }
    /// <inheritdoc/>
    public int ClientCount => ClientStreamerConfigurations.Keys.Count;
    /// <inheritdoc/>
    public List<IClientConfiguration> GetClientStreamerConfigurations()
    {
        return [.. ClientStreamerConfigurations.Values];
    }


    /// <inheritdoc/>
    public bool IsFailed { get; set; } = false;

    /// <inheritdoc/>
    public string Name { get; set; } = string.Empty;

    /// <inheritdoc/>
    [XmlIgnore]
    public string SourceName { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public StreamHandlerMetrics Metrics => new()
    {
        BytesRead = GetBytesRead(),
        Kbps = GetKbps(),
        StartTime = GetStartTime(),
        AverageLatency = GetAverageLatency()
    };

    /// <inheritdoc/>
    public void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader, string sourceChannelName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source channel for {Name} to {sourceChannelName}", Name, sourceChannelName);
        SourceName = sourceChannelName;
        Channel<byte[]> newChannel = ChannelHelper.GetChannel();
        //_currentChannel?.Writer.TryComplete();
        StartProcessingSource(sourceChannelReader, null, newChannel, cancellationToken);
    }

    /// <inheritdoc/>
    public void SetSourceStream(Stream sourceStream, string streamName, CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting source stream to {streamName}", streamName);
        SourceName = streamName;
        Channel<byte[]> newChannel = ChannelHelper.GetChannel();

        StartProcessingSource(null, sourceStream, newChannel, cancellationToken);
    }

    /// <inheritdoc/>
    public void Stop()
    {
        if (!_cancellationTokenSource.IsCancellationRequested)
        {
            logger.LogInformation("Stopped broadcaster for: {Id} {Name}", StringId(), Name);
            Debug.WriteLine($"Broadcaster had {StringId()} {Name} {ChannelItemBackLog} left");
            _cancellationTokenSource.Cancel();
        }

        if (!ClientChannelWriters.IsEmpty)
        {
            foreach (KeyValuePair<string, ChannelWriter<byte[]>> client in ClientChannelWriters)
            {

                RemoveChannelStreamer(client.Key);
            }
            return;
        }
    }


    //Channels
    /// <inheritdoc/>
    public void AddChannelStreamer(int smChannelId, ChannelWriter<byte[]> channel)
    {
        AddChannelStreamer(smChannelId.ToString(), channel);
    }
    /// <inheritdoc/>
    public bool RemoveChannelStreamer(int smChannelId)
    {
        return RemoveChannelStreamer(smChannelId.ToString());
    }

    //Plugins

    /// <inheritdoc/>
    public void AddChannelStreamer(string UniqueRequestId, ChannelWriter<byte[]> channel)
    {
        logger.LogInformation("Add channel streamer: {UniqueRequestId} to {Name}", UniqueRequestId, Name);
        ClientChannelWriters.TryAdd(UniqueRequestId, channel);
    }

    /// <inheritdoc/>
    public bool RemoveChannelStreamer(string UniqueRequestId)
    {
        if (ClientChannelWriters.TryRemove(UniqueRequestId, out _))
        {
            logger.LogInformation("Remove channel streamer: {UniqueRequestId} {Name}", UniqueRequestId, Name);
            return true;
        }
        return false;
    }

    //Clients
    /// <inheritdoc/>
    public void AddClientStreamer(string UniqueRequestId, IClientConfiguration config)
    {
        if (ClientStreamerConfigurations.TryAdd(UniqueRequestId, config))
        {
            logger.LogInformation("Add client streamer: {UniqueRequestId} to {Name}", UniqueRequestId, Name);
            ClientChannelWriters.TryAdd(UniqueRequestId, config.ClientStream!.Channel);
        }
    }

    /// <inheritdoc/>
    public bool RemoveClientStreamer(string UniqueRequestId)
    {
        if (ClientStreamerConfigurations.TryRemove(UniqueRequestId, out IClientConfiguration? clientConfiguration) && ClientChannelWriters.TryRemove(UniqueRequestId, out _))
        {
            logger.LogInformation("Remove client streamer: {UniqueRequestId} {Name}", UniqueRequestId, Name);

            clientConfiguration.Stop();
            //if (ClientChannelWriters.TryRemove(UniqueRequestId, out _))
            //{
            //    logger.LogInformation("Remove client streamer: {UniqueRequestId} {Name}", UniqueRequestId, Name);
            //}
            return true;
        }

        return false;
    }

    private void StartProcessingSource(ChannelReader<byte[]>? sourceChannelReader, Stream? sourceStream, Channel<byte[]> newChannel, CancellationToken cancellationToken)
    {
        CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        CancellationToken token = linkedTokenSource.Token;
        Task readerTask = Task.CompletedTask;
        Task writerTask;

        if (sourceChannelReader != null)
        {
            readerTask = ProcessSourceChannelReader(sourceChannelReader, newChannel, token);
        }
        else if (sourceStream != null)
        {
            readerTask = ProcessSourceStream(sourceStream, newChannel, token);
        }

        writerTask = DistributeToClients(newChannel, token);
        _ = MonitorTasksCompletion(readerTask, writerTask, sourceStream, token);
    }

    private Task ProcessSourceChannelReader(ChannelReader<byte[]> sourceChannelReader, Channel<byte[]> newChannel, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            bool localInputStreamError = false;
            try
            {
                await foreach (byte[] item in sourceChannelReader.ReadAllAsync(token))
                {
                    //Debug.WriteLine($"ProcessSourceChannelReader stream {Name}");
                    await newChannel.Writer.WriteAsync(item, token).ConfigureAwait(false);
                    Interlocked.Increment(ref _channelItemCount);

                    SetMetrics(item.Length, 0);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Stream reading canceled for: {id} {name}", StringId(), Name);
            }
            catch (Exception ex)
            {
                HandleReaderExceptions(ex, ref localInputStreamError);
            }
            finally
            {
                newChannel.Writer.TryComplete();
            }
        }, token);
    }

    private Task ProcessSourceStream(Stream sourceStream, Channel<byte[]> newChannel, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            bool localInputStreamError = false;
            try
            {
                byte[] buffer = new byte[16384];
                while (!token.IsCancellationRequested)
                {
                    Stopwatch sw = Stopwatch.StartNew();

                    int bytesRead = await sourceStream.ReadAsync(buffer, token).ConfigureAwait(false);
                    sw.Stop();
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    byte[] data = new byte[bytesRead];
                    Array.Copy(buffer, data, bytesRead);
                    await newChannel.Writer.WriteAsync(data, token).ConfigureAwait(false);
                    Interlocked.Increment(ref _channelItemCount);

                    SetMetrics(bytesRead, sw.Elapsed.TotalMilliseconds);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Stream reading canceled for: {id} {name}", StringId(), Name);
            }
            catch (Exception ex)
            {
                HandleReaderExceptions(ex, ref localInputStreamError);
            }
            finally
            {
                newChannel.Writer.TryComplete();
            }
        }, token);
    }

    private void HandleReaderExceptions(Exception ex, ref bool inputStreamError)
    {
        switch (ex)
        {
            case TaskCanceledException _:
            case OperationCanceledException _:
                logger.LogInformation(ex, "Stream reading canceled for: {id} {name}", StringId(), Name);
                break;
            case EndOfStreamException _:
                logger.LogInformation(ex, "End of stream reached for: {id} {name}", StringId(), Name);
                inputStreamError = true;
                break;
            case HttpIOException _:
                logger.LogInformation(ex, "HTTP I/O exception for: {id} {name}", StringId(), Name);
                inputStreamError = true;
                break;
            default:
                logger.LogError(ex, "Unexpected error for: {name}", Name);
                break;
        }
    }
    private Task DistributeToClients(Channel<byte[]> newChannel, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                await foreach (byte[] item in newChannel.Reader.ReadAllAsync(token))
                {
                    List<Task> writeTasks = [];
                    foreach (ChannelWriter<byte[]> clientChannel in ClientChannelWriters.Values)
                    {
                        writeTasks.Add(clientChannel.WriteAsync(item, token).AsTask());
                    }
                    //foreach (Stream clientStream in ClientStreams.Values)
                    //{
                    //    //Stream clientStream = ClientStreams[key];
                    //    writeTasks.Add(clientStream.WriteAsync(item, 0, item.Length, token));
                    //}
                    await Task.WhenAll(writeTasks).ConfigureAwait(false);
                    Interlocked.Decrement(ref _channelItemCount);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Stream operation canceled or ended for: {name}", Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error during stream reading for: {name}", Name);
            }
        }, token);
    }

    private Task MonitorTasksCompletion(Task readerTask, Task writerTask, Stream? sourceStream, CancellationToken token)
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
                OnBaseStopped();
            }
        }, token);
    }
    /// <inheritdoc/>
    public virtual void OnBaseStopped()
    {
    }
    /// <inheritdoc/>
    public virtual string StringId()
    {
        return "NA";
    }

    private void SetMetrics(int bytesRead, double latency)
    {
        Interlocked.Add(ref _bytesRead, bytesRead);
        _bytesReadCounter.Add(bytesRead);
        _latencyHistogram.Record(latency);
        _bpsStatistics.AddBytesRead(bytesRead);
        double kbps = GetKbps();
        _kbpsHistogram.Record(kbps);
        // Add latency to the queue
        _latencies.Enqueue(latency);
        // Maintain a fixed size for the latency queue
        while (_latencies.Count > 100) // Adjust this number as needed
        {
            _latencies.TryDequeue(out _);
        }
    }

    /// <inheritdoc/>
    public long GetBytesRead()
    {
        return Interlocked.Read(ref _bytesRead);
    }

    /// <inheritdoc/>
    public double GetAverageLatency()
    {
        return _latencies.IsEmpty ? 0 : _latencies.Average();
    }

    /// <inheritdoc/>
    public double GetKbps()
    {
        return _bpsStatistics.BitsPerSecond / 1000.0;
    }

    /// <inheritdoc/>
    public DateTime GetStartTime()
    {
        return _startTime;
    }

    /// <inheritdoc/>
    public long ChannelItemBackLog => Interlocked.Read(ref _channelItemCount);

    /// <inheritdoc/>
    public bool IsChannelEmpty()
    {
        return ChannelItemBackLog == 0;
    }
}
