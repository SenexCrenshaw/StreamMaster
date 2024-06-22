using StreamMaster.Domain.Extensions;

using System.Diagnostics;

namespace StreamMaster.Streams.Streams;

/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed partial class StreamHandler
{
    private const int videoBufferSize = 1 * 1024 * 1000;
    public const int ChunkSize = 64 * 1024;

    private readonly Memory<byte> videoBuffer = new byte[videoBufferSize];

    private DateTime LastVideoInfoRun = DateTime.MinValue;


    private async Task WriteToAllClientsAsync(byte[] data, CancellationToken cancellationToken)
    {
        IEnumerable<Task> tasks = clientStreamerConfigs.Values
            .Where(c => c.ClientStream?.Channel?.Writer != null)
            .Select(async clientStreamerConfig =>
            {
                try
                {
                    await clientStreamerConfig.ClientStream.Channel.Writer.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to write to client {ClientId}", clientStreamerConfig.ClientId);
                }
            });

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while writing to all clients.");
        }
    }

    public async Task StartVideoStreamingAsync(Stream stream)
    {
        VideoStreamingCancellationToken = new();

        logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl} name: {name}", ChunkSize, SMStream.Url, SMStream.Name);

        bool inputStreamError = false;
        CancellationTokenSource linkedToken = VideoStreamingCancellationToken;
        bool ran = false;
        int accumulatedBytes = 0;

        Stopwatch testSw = Stopwatch.StartNew();

        Memory<byte> bufferMemory = new byte[ChunkSize];
        List<byte> intervalBuffer = new List<byte>();
        Task? runTask = null;

        using (stream)
        {
            while (!linkedToken.IsCancellationRequested)
            {
                if (ClientCount == 0)
                {
                    if (ran)
                    {
                        logger.LogWarning("No more clients, breaking");
                        break;
                    }
                    await Task.Delay(10).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    _writeLogger.LogDebug("-------------------{VideoStreamName}-----------------------------", SMStream.Name);
                    int readBytes = await stream.ReadAsync(bufferMemory, linkedToken.Token).ConfigureAwait(false);
                    _writeLogger.LogDebug("End bytes read from input stream: {byteswritten}", readBytes);
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

                    byte[] clientDataToSend = new byte[readBytes];
                    bufferMemory[..readBytes].CopyTo(clientDataToSend);

                    intervalBuffer.AddRange(clientDataToSend);
                    accumulatedBytes += readBytes;


                    await WriteToAllClientsAsync(clientDataToSend, linkedToken.Token).ConfigureAwait(false);

                    if (intervalBuffer.Count > videoBufferSize)
                    {
                        // Keep only the latest videoBufferSize bytes
                        intervalBuffer = intervalBuffer.Skip(intervalBuffer.Count - videoBufferSize).ToList();
                    }


                    if (runTask == null || runTask.IsCompleted)
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
                    SetMetrics(readBytes);

                }
                catch (TaskCanceledException)
                {
                    logger.LogInformation("Stream requested to stop for: {StreamUrl} {name}", SMStream.Url, SMStream.Name);
                    logger.LogInformation("Stream requested to stop for: {VideoStreamingCancellationToken}", VideoStreamingCancellationToken.IsCancellationRequested);
                    break;
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Stream Operation stopped for: {StreamUrl} {name}", SMStream.Url, SMStream.Name);
                    break;
                }
                catch (EndOfStreamException ex)
                {
                    inputStreamError = true;
                    logger.LogInformation("End of Stream reached for: {StreamUrl} {name}. Error: {ErrorMessage} at {Time} {test}", SMStream.Url, SMStream.Name, ex.Message, SMDT.UtcNow, stream.CanRead);
                    break;
                }
                catch (HttpIOException ex)
                {
                    inputStreamError = true;
                    logger.LogInformation(ex, "HTTP IO for: {StreamUrl} {name}", SMStream.Url, SMStream.Name);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Stream error for: {StreamUrl} {name}", SMStream.Url, SMStream.Name);
                    break;
                }
            }
        }

        IsFailed = true;
        stream.Close();
        stream.Dispose();

        OnStreamingStopped(inputStreamError);
    }
}
