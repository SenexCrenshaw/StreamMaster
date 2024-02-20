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

        Memory<byte> test = new(new byte[videoBufferSize]);

        byte[] firstByte = new byte[videoBufferSize];
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
                    Memory<byte> bufferMemory = new byte[ChunkSize];
                    _writeLogger.LogDebug("-------------------{VideoStreamName}-----------------------------", VideoStreamName);
                    int readBytes = await stream.ReadAsync(bufferMemory, linkedToken.Token);
                    _writeLogger.LogDebug("End bytes written: {byteswritten}", readBytes);
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

                    foreach (IClientStreamerConfiguration clientStreamerConfig in clientStreamerConfigs.Values)
                    {
                        if (clientStreamerConfig.ReadBuffer != null)
                        {
                            await clientStreamerConfig.ReadBuffer.ReadChannel.Writer.WriteAsync(clientDataToSend);
                        }
                        else
                        {
                            logger.LogError("ClientStreamerConfig ReadBuffer is null for {ClientId}", clientStreamerConfig.ClientId);
                        }

                    }

                    TimeSpan lastRun = SMDT.UtcNow - LastVideoInfoRun;
                    if (lastRun.TotalMinutes >= 30)
                    {
                        if (accumulatedBytes > videoBufferSize)
                        {
                            // Calculate the amount of data to process now, which is the size of the video buffer.
                            int dataToProcessNow = videoBufferSize;

                            // Calculate the overage, which is the total accumulated bytes minus what we're processing now.
                            int overage = accumulatedBytes - dataToProcessNow;

                            // Process the data up to `dataToProcessNow`. This part remains as is, assuming you have logic to handle this.
                            byte[] processData = new byte[dataToProcessNow];
                            Array.Copy(clientDataToSend, processData, dataToProcessNow);
                            Task task = BuildVideoInfoAsync(processData);

                            // Now handle the overage.
                            // Instead of directly writing to `videoBuffer`, adjust your logic to manage the overage bytes.
                            // Since `clientDataToSend` might not directly correspond to `videoBuffer` contents,
                            // ensure you have a way to reference the correct segment of overage data.
                            if (overage > 0)
                            {
                                byte[] overageData = new byte[overage];
                                Array.Copy(clientDataToSend, dataToProcessNow, overageData, 0, overage);
                                videoBuffer.Write(overageData); // Write the overage back into the buffer
                            }

                            // Reset `accumulatedBytes` to reflect only the overage, since everything else has been processed.
                            accumulatedBytes = overage;
                        }
                        else
                        {
                            // If accumulated bytes are within the buffer size, process as usual.
                            Task task = BuildVideoInfoAsync(clientDataToSend);
                            // After processing, reset accumulatedBytes as all data has been handled.
                            accumulatedBytes = 0;
                        }
                    }
                    else
                    {
                        // For regular writes outside the 30-minute check.
                        videoBuffer.Write(clientDataToSend);
                        accumulatedBytes += readBytes;
                    }

                    //if (lastRun.TotalMinutes >= 30 && accumulatedBytes + readBytes > videoBufferSize)
                    //{

                    //    int overAge = accumulatedBytes + readBytes - videoBufferSize;
                    //    int toRead = readBytes - overAge;
                    //    if (toRead < 0)
                    //    {
                    //        logger.LogError(overAge, "toRead is less than {overAge}", overAge);
                    //        logger.LogError(overAge, "accumulatedBytes {accumulatedBytes}", accumulatedBytes);
                    //        logger.LogError(overAge, "readBytes {readBytes}", readBytes);
                    //        logger.LogError(overAge, "readBytes {videoBufferSize}", videoBufferSize);
                    //    }
                    //    else
                    //    {
                    //        videoBuffer.Write(clientDataToSend[..toRead]);

                    //        byte[] videoMemory = videoBuffer.ReadLatestData();
                    //        Task task = BuildVideoInfoAsync(videoMemory);

                    //        ++toRead;
                    //        accumulatedBytes = readBytes - toRead;
                    //        videoBuffer.Write(clientDataToSend[toRead..readBytes]);
                    //    }
                    //}
                    //else
                    //{
                    //    videoBuffer.Write(clientDataToSend);
                    //    accumulatedBytes += readBytes;
                    //}
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
        //    clientStreamerConfig.ReadBuffer?.ReadChannel.Writer.Complete();
        //}
        IsFailed = true;
        stream.Close();
        stream.Dispose();

        OnStreamingStopped(inputStreamError || (timeOutToken != null && timeOutToken.IsCancellationRequested && !testRan));
        testRan = true;
    }

}