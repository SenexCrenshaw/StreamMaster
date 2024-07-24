using StreamMaster.Domain.Extensions;
using StreamMaster.Streams.Buffers;

using System.Diagnostics;
using System.Threading.Channels;

namespace StreamMaster.Streams.Streams
{

    public sealed partial class StreamHandler
    {
        private const int videoBufferSize = 1 * 1024 * 1000;
        public const int ChunkSize = 64 * 1024;
        private DateTime LastVideoInfoRun = DateTime.MinValue;
        //public async Task StartVideo(IChannelStatus channelStatus)
        //{

        //    (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(channelStatus, cancellationToken).ConfigureAwait(false);
        //    //if (stream == null || error != null || processId == 0)
        //    //{
        //    //    return null;
        //    //}

        //    //StreamHandler streamHandler = new(channelStatus.SMStream, processId, intSettings, loggerFactory);

        //    //_ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream), cancellationToken);
        //}

        public async Task StartVideoStreamingAsync(Stream stream)
        {
            VideoStreamingCancellationToken = new CancellationTokenSource();
            logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream name: {name}", ChunkSize, SMStream.Name);

            bool inputStreamError = false;
            CancellationTokenSource linkedToken = VideoStreamingCancellationToken;
            bool ran = false;
            int accumulatedBytes = 0;

            Stopwatch testSw = Stopwatch.StartNew();
            Memory<byte> bufferMemory = new byte[ChunkSize];

            Task? runTask = null;
            Channel<byte[]> channel = Channel.CreateUnbounded<byte[]>();

            async Task Reader(ChannelWriter<byte[]> writer, CancellationToken token)
            {
                try
                {
                    using (stream)
                    {
                        while (!token.IsCancellationRequested)
                        {

                            Stopwatch readStart = Stopwatch.StartNew();
                            int readBytes = await stream.ReadAsync(bufferMemory, token).ConfigureAwait(false);
                            readStart.Stop();
                            double latency = readStart.Elapsed.TotalMilliseconds;

                            if (readBytes == 0)
                            {
                                throw new EndOfStreamException();
                            }

                            if (!ran)
                            {
                                ran = true;
                                testSw.Stop();
                                logger.LogInformation("Input stream took {ElapsedMilliseconds}ms before reading first bytes {readBytes}", testSw.ElapsedMilliseconds, readBytes);
                            }

                            byte[] data = bufferMemory[..readBytes].ToArray();
                            await writer.WriteAsync(data, token).ConfigureAwait(false);

                            SetMetrics(readBytes, 0, latency);
                        }
                    }
                }
                catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException or EndOfStreamException or HttpIOException)
                {
                    inputStreamError = true;
                    logger.LogInformation(ex, "Stream stopped for: {StreamUrl} {name}", SMStream.Url, SMStream.Name);
                    ErrorCounter.Add(1);
                    _ = Interlocked.Increment(ref errorCount);
                    lastErrorTime = DateTime.UtcNow;
                }
                finally
                {
                    writer.Complete();
                }
            }

            async Task Writer(ChannelReader<byte[]> reader, CancellationToken token)
            {
                CircularBuffer<byte> intervalBuffer = new(videoBufferSize);

                try
                {
                    await foreach (byte[]? data in reader.ReadAllAsync(token).ConfigureAwait(false))
                    {
                        await _outputChannel.Writer.WriteAsync(data, token);

                        SetMetrics(0, data.Length, 0);

                        foreach (byte b in data)
                        {
                            intervalBuffer.Enqueue(b);
                        }

                        accumulatedBytes += data.Length;

                        if (runTask?.IsCompleted != false)
                        {
                            if (accumulatedBytes >= videoBufferSize)
                            {
                                TimeSpan lastRun = SMDT.UtcNow - LastVideoInfoRun;
                                if (_videoInfo == null || lastRun.TotalMinutes >= 10)
                                {
                                    runTask = BuildVideoInfoAsync(intervalBuffer.ToArray());

                                    // Reset the buffer and accumulated bytes after triggering the task
                                    accumulatedBytes = 0;
                                    intervalBuffer.Clear();
                                }
                            }
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    ErrorCounter.Add(1);
                    _ = Interlocked.Increment(ref errorCount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Processing error for: {StreamUrl} {name}", SMStream.Url, SMStream.Name);
                    ErrorCounter.Add(1);
                    _ = Interlocked.Increment(ref errorCount);
                }
            }

            try
            {
                Task readerTask = Reader(channel.Writer, linkedToken.Token);
                Task writerTask = Writer(channel.Reader, linkedToken.Token);
                await Task.WhenAll(readerTask, writerTask).ConfigureAwait(false);
            }
            finally
            {
                IsFailed = true;
                if (stream is IAsyncDisposable asyncDisposableStream)
                {
                    await asyncDisposableStream.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    stream.Dispose();
                }

                OnStreamingStopped(inputStreamError);
            }
        }
    }
}
