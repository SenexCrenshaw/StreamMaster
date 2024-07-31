using StreamMaster.Streams.Domain.Statistics;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Handlers
{

    /// <summary>
    /// The ChannelDistributor class is responsible for multiplexing a source channel to multiple client channels.
    /// </summary>
    public class ChannelDistributor : IChannelDistributor
    {
        public event EventHandler<ChannelDirectorStopped>? OnStoppedEvent;
        public SMStreamInfo SMStreamInfo { get; }
        public ConcurrentDictionary<string, Stream> ClientStreams { get; } = new();

        private const string toCheck = "sdjjddj";
        private readonly Meter _meter;
        private readonly Counter<long> _bytesReadCounter;
        private readonly Histogram<double> _kbpsHistogram;
        private readonly Histogram<double> _latencyHistogram;
        private readonly BPSStatistics _bpsStatistics = new();
        private long _bytesRead;
        private long _channelItemCount;
        private readonly DateTime _startTime = DateTime.UtcNow;
        private readonly ConcurrentQueue<double> _latencies = new();
        private readonly ILogger<IChannelDistributor> _logger;
        private readonly Channel<byte[]> _currentChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public void AddClientStream(string key, Stream stream)
        {
            ClientStreams.TryAdd(key, stream);
        }

        public void AddClientStream(int key, Stream stream)
        {
            ClientStreams.TryAdd(key.ToString(), stream);
        }

        public bool RemoveClientStream(string key)
        {
            return ClientStreams.TryRemove(key, out _);
        }

        public bool RemoveClientStream(int key)
        {
            return ClientStreams.TryRemove(key.ToString(), out _);
        }

        public StreamHandlerMetrics GetMetrics => new()
        {
            BytesRead = GetBytesRead(),
            Kbps = GetKbps(),
            StartTime = GetStartTime(),
            AverageLatency = GetAverageLatency()
        };


        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelDistributor"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="smStreamInfo">SMStreamInfo of the distributor.</param>
        public ChannelDistributor(ILogger<IChannelDistributor> logger, SMStreamInfo smStreamInfo)
        {
            SourceName = smStreamInfo.Name;
            _logger = logger;
            SMStreamInfo = smStreamInfo;
            _meter = new Meter("StreamHandlerMetrics", "1.0");
            _bytesReadCounter = _meter.CreateCounter<long>("bytes_read");
            _kbpsHistogram = _meter.CreateHistogram<double>("kbps", "kbps");
            _latencyHistogram = _meter.CreateHistogram<double>("latency", "ms");
        }

        public ChannelDistributor(ILogger<IChannelDistributor> logger, SMChannelDto smChannelDto)
        {
            SourceName = smChannelDto.Name;
            _logger = logger;
            SMStreamInfo = SMStreamInfo.NewSMStreamInfo(smChannelDto.Name, smChannelDto.IsCustomStream);
            _meter = new Meter("StreamHandlerMetrics", "1.0");
            _bytesReadCounter = _meter.CreateCounter<long>("bytes_read");
            _kbpsHistogram = _meter.CreateHistogram<double>("kbps", "kbps");
            _latencyHistogram = _meter.CreateHistogram<double>("latency", "ms");
        }

        /// <summary>
        /// Gets the dictionary of client channels.
        /// </summary>
        public ConcurrentDictionary<string, ChannelWriter<byte[]>> ClientChannels { get; } = new();

        public bool IsFailed { get; set; } = false;

        public string SourceName { get; private set; }

        /// <summary>
        /// Sets a new source channel and starts processing it.
        /// </summary>
        /// <param name="sourceChannelReader">The source channel reader.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        public void SetSourceChannel(ChannelReader<byte[]> sourceChannelReader, string Name, CancellationToken cancellationToken)
        {
            SourceName = Name;
            Channel<byte[]> newChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

            // Stop the current channel processing
            _currentChannel.Writer.TryComplete();

            // Start processing the new source channel
            StartProcessingSource(sourceChannelReader, null, newChannel, cancellationToken);
        }

        /// <summary>
        /// Sets a new source stream and starts processing it.
        /// </summary>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
        public void SetSourceStream(Stream sourceStream, string Name, CancellationToken cancellationToken)
        {
            SourceName = Name;
            Channel<byte[]> newChannel = Channel.CreateUnbounded<byte[]>(new UnboundedChannelOptions { SingleReader = true, SingleWriter = true });

            // Stop the current channel processing
            _currentChannel.Writer.TryComplete();

            // Start processing the new source stream
            StartProcessingSource(null, sourceStream, newChannel, cancellationToken);
        }

        /// <summary>
        /// Stops the channel distributor by canceling the internal cancellation token source.
        /// </summary>
        public void Stop()
        {
            Debug.WriteLine($"Dist had {GetChannelItemCount} left");
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Adds a client channel to the dictionary.
        /// </summary>
        /// <param name="key">The key for the client channel.</param>
        /// <param name="channel">The client channel to add.</param>
        public void AddClientChannel(string key, ChannelWriter<byte[]> channel)
        {
            ClientChannels.TryAdd(key, channel);
        }

        /// <summary>
        /// Adds a client channel to the dictionary.
        /// </summary>
        /// <param name="key">The key for the client channel as an integer.</param>
        /// <param name="channel">The client channel to add.</param>
        public void AddClientChannel(int key, ChannelWriter<byte[]> channel)
        {
            ClientChannels.TryAdd(key.ToString(), channel);
        }

        /// <summary>
        /// Removes a client channel from the dictionary.
        /// </summary>
        /// <param name="key">The key for the client channel.</param>
        /// <returns>True if the client channel was successfully removed; otherwise, false.</returns>
        public bool RemoveClientChannel(string key)
        {
            return ClientChannels.TryRemove(key, out _);
        }

        /// <summary>
        /// Removes a client channel from the dictionary.
        /// </summary>
        /// <param name="key">The key for the client channel as an integer.</param>
        /// <returns>True if the client channel was successfully removed; otherwise, false.</returns>
        public bool RemoveClientChannel(int key)
        {
            return ClientChannels.TryRemove(key.ToString(), out _);
        }

        /// <summary>
        /// Starts processing the source channel or stream and writes data to the new channel.
        /// </summary>
        /// <param name="sourceChannelReader">The source channel reader.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="newChannel">The new channel to write data to.</param>
        /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
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
                        await newChannel.Writer.WriteAsync(item, token).ConfigureAwait(false);
                        Interlocked.Increment(ref _channelItemCount);
                        if (SMStreamInfo.Name == toCheck)
                        {
                            Debug.WriteLine($"{_channelItemCount}");
                        }
                        SetMetrics(item.Length, 0);
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Stream reading canceled for: {name}", SMStreamInfo);
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
                    byte[] buffer = new byte[8192]; // Buffer size can be adjusted as needed
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
                        if (SMStreamInfo.Name == toCheck)
                        {
                            Debug.WriteLine($"{_channelItemCount}");
                        }
                        SetMetrics(bytesRead, sw.Elapsed.TotalMilliseconds); // Update metrics with latency
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Stream reading canceled for: {name}", SMStreamInfo);
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
                    _logger.LogInformation(ex, "Stream reading canceled for: {name}", SMStreamInfo);
                    break;
                case EndOfStreamException _:
                    _logger.LogInformation(ex, "End of stream reached for: {name}", SMStreamInfo);
                    inputStreamError = true;
                    break;
                case HttpIOException _:
                    _logger.LogInformation(ex, "HTTP I/O exception for: {name}", SMStreamInfo);
                    inputStreamError = true;
                    break;
                default:
                    _logger.LogError(ex, "Unexpected error for: {name}", SMStreamInfo);
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

                        foreach (ChannelWriter<byte[]> clientChannel in ClientChannels.Values)
                        {
                            writeTasks.Add(clientChannel.WriteAsync(item, token).AsTask());
                        }

                        foreach (Stream clientStream in ClientStreams.Values)
                        {
                            writeTasks.Add(clientStream.WriteAsync(item, 0, item.Length, token));
                        }

                        await Task.WhenAll(writeTasks).ConfigureAwait(false);
                        Interlocked.Decrement(ref _channelItemCount);
                        if (SMStreamInfo.Name == toCheck)
                        {
                            Debug.WriteLine($"{_channelItemCount}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Stream operation canceled or ended for: {name}", SMStreamInfo);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error during stream reading for: {name}", SMStreamInfo);
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
                    OnStreamingStopped();
                }
            }, token);
        }

        private void OnStreamingStopped()
        {
            OnStoppedEvent?.Invoke(this, new ChannelDirectorStopped(SMStreamInfo));
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

        public long GetBytesRead()
        {
            return Interlocked.Read(ref _bytesRead);
        }

        public double GetAverageLatency()
        {
            return _latencies.IsEmpty ? 0 : _latencies.Average();
        }

        public double GetKbps()
        {
            return _bpsStatistics.BitsPerSecond / 1000.0;
        }

        public DateTime GetStartTime()
        {
            return _startTime;
        }

        public long GetChannelItemCount => Interlocked.Read(ref _channelItemCount);


        public bool IsChannelEmpty()
        {
            return GetChannelItemCount == 0;
        }
    }
}
