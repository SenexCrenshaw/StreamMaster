using Microsoft.Extensions.Logging;

using StreamMaster.Domain;
using StreamMaster.Domain.Cache;
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

    private readonly CircularBuffer videoBuffer = new(videoBufferSize);
    private bool testRan = false;

    private DateTime LastVideoInfoRun = DateTime.MinValue;

    // Write to all clients with separate buffers
    private async Task WriteToAllClientsAsync(byte[] data, CancellationToken cancellationToken)
    {
        IEnumerable<Task> tasks = clientStreamerConfigs.Values
            .Where(c => c.Stream != null)
            .Select(async clientStreamerConfig =>
            {
                if (clientStreamerConfig.Stream != null)
                {
                    try
                    {
                        await clientStreamerConfig.Stream.Channel.Writer.WriteAsync(data, cancellationToken).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Failed to write to client {ClientId}", clientStreamerConfig.ClientId);
                    }
                }
            });

        await Task.WhenAll(tasks);
    }

    public async Task StartVideoStreamingAsync(Stream stream)
    {

        VideoStreamingCancellationToken = new();

        logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl} name: {name}", ChunkSize, StreamUrl, VideoStreamName);

        bool inputStreamError = false;

        CancellationTokenSource linkedToken;
        CancellationTokenSource? timeOutToken = null;

        if (!testRan && memoryCache.GetSetting().TestSettings.DropInputSeconds > 0)
        {
            timeOutToken = new();
            logger.LogInformation("Testing: Will stop stream in {DropInputSeconds} seconds.", memoryCache.GetSetting().TestSettings.DropInputSeconds);
            timeOutToken.CancelAfter(memoryCache.GetSetting().TestSettings.DropInputSeconds * 1000);
            linkedToken = CancellationTokenSource.CreateLinkedTokenSource(VideoStreamingCancellationToken.Token, timeOutToken.Token);
        }
        else
        {
            linkedToken = VideoStreamingCancellationToken;
        }

        bool ran = false;
        int accumulatedBytes = 0;
        Stopwatch testSw = Stopwatch.StartNew();
        Memory<byte> bufferMemory = new byte[ChunkSize];

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
                    await Task.Delay(10);
                    continue;
                }

                try
                {
                    _writeLogger.LogDebug("-------------------{VideoStreamName}-----------------------------", VideoStreamName);
                    int readBytes = await stream.ReadAsync(bufferMemory, linkedToken.Token);
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
                    await WriteToAllClientsAsync(clientDataToSend, linkedToken.Token).ConfigureAwait(false);

                    videoBuffer.Write(clientDataToSend);
                    accumulatedBytes += readBytes;

                    TimeSpan lastRun = SMDT.UtcNow - LastVideoInfoRun;
                    if (lastRun.TotalMinutes >= 3)
                    {
                        if (accumulatedBytes > videoBufferSize)
                        {
                            var processData = videoBuffer.ReadLatestData();
                            _ = BuildVideoInfoAsync(processData);

                            accumulatedBytes = 0;
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    logger.LogInformation("Stream requested to stop for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    logger.LogInformation("Stream requested to stop for: {VideoStreamingCancellationToken}", VideoStreamingCancellationToken.IsCancellationRequested);
                    break;
                }
                catch (OperationCanceledException)
                {

                    logger.LogInformation("Stream Operation stopped for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    break;
                }
                catch (EndOfStreamException ex)
                {
                    inputStreamError = true;
                    logger.LogInformation("End of Stream reached for: {StreamUrl} {name}. Error: {ErrorMessage} at {Time} {test}", StreamUrl, VideoStreamName, ex.Message, SMDT.UtcNow, stream.CanRead);
                    break;
                }
                catch (HttpIOException ex)
                {
                    inputStreamError = true;
                    logger.LogInformation(ex, "HTTP IO for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    break;
                }

                catch (Exception ex)
                {

                    logger.LogError(ex, "Stream error for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    break;
                }
            }
        }

        //foreach (IClientStreamerConfiguration clientStreamerConfig in clientStreamerConfigs.Values)
        //{
        //    clientStreamerConfig.Stream?.Channel.Writer.Complete();
        //}
        IsFailed = true;
        stream.Close();
        stream.Dispose();

        OnStreamingStopped(inputStreamError || (timeOutToken != null && timeOutToken.IsCancellationRequested && !testRan));
        testRan = true;
    }

}