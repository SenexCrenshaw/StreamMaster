using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Dto;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMasterInfrastructure.VideoStreamManager.Streams;

/// <summary>
/// Manages the streaming of a single video stream, including client registrations and circularRingbuffer handling.
/// </summary>
public sealed class StreamHandler(VideoStreamDto videoStreamDto, int processId, ILogger<IStreamHandler> logger, ICircularRingBuffer ringBuffer) : IStreamHandler
{
    public event EventHandler<string> OnStreamingStoppedEvent;

    private readonly ConcurrentDictionary<Guid, Guid> clientStreamerIds = new();

    public int M3UFileId { get; } = videoStreamDto.M3UFileId;
    public int ProcessId { get; set; } = processId;
    public ICircularRingBuffer CircularRingBuffer { get; } = ringBuffer;
    public string StreamUrl { get; } = videoStreamDto.User_Url;
    public string VideoStreamId { get; } = videoStreamDto.Id;
    public string VideoStreamName { get; } = videoStreamDto.User_Tvg_name;

    private CancellationTokenSource VideoStreamingCancellationToken { get; set; } = new();

    public int ClientCount => clientStreamerIds.Count;

    public bool IsFailed { get; private set; }

    private void OnStreamingStopped()
    {
        OnStreamingStoppedEvent?.Invoke(this, StreamUrl);
    }

    //private async Task DelayWithCancellation(int milliseconds)
    //{
    //    try
    //    {
    //        await Task.Delay(milliseconds, VideoStreamingCancellationToken.Token);
    //    }
    //    catch (TaskCanceledException)
    //    {
    //        logger.LogInformation("Task was cancelled");
    //        throw;
    //    }
    //}

    //private async Task LogRetryAndDelay(int retryCount, int maxRetries, int waitTime)
    //{
    //    if (VideoStreamingCancellationToken.Token.IsCancellationRequested)
    //    {
    //        logger.LogInformation("Stream was cancelled for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
    //    }

    //    logger.LogInformation("Stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries} {name}",
    //        StreamUrl,
    //        retryCount,
    //        maxRetries, VideoStreamName);

    //    await DelayWithCancellation(waitTime);
    //}

    public async Task StartVideoStreamingAsync(Stream stream, ICircularRingBuffer circularRingbuffer)
    {
        const int chunkSize = 64 * 1024;

        logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl} name: {name} circularRingbuffer id: {circularRingbuffer}", chunkSize, StreamUrl, VideoStreamName, circularRingbuffer.Id);

        Memory<byte> bufferMemory = new byte[chunkSize];

        //const int maxRetries = 3;
        //const int waitTime = 50;

        using (stream)
        {
            while (!VideoStreamingCancellationToken.IsCancellationRequested)// && retryCount < maxRetries)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(bufferMemory);
                    if (bytesRead < 1)
                    {
                        break;
                    }
                    else
                    {
                        circularRingbuffer.WriteChunk(bufferMemory[..bytesRead]);
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Stream error for: {StreamUrl} {name}", StreamUrl, VideoStreamName);
                    break;
                }
            }
        }

        stream.Close();
        stream.Dispose();

        logger.LogInformation("Stream stopped for: {StreamUrl} {name}", StreamUrl, VideoStreamName);

        OnStreamingStopped();
    }



    public void Dispose()
    {
        clientStreamerIds.Clear();
        Stop();
        GC.SuppressFinalize(this);
    }

    public void Stop()
    {
        if (VideoStreamingCancellationToken?.IsCancellationRequested == false)
        {
            VideoStreamingCancellationToken.Cancel();
        }

        if (ProcessId > 0)
        {
            try
            {
                string? procName = CheckProcessExists();
                if (procName != null)
                {
                    Process process = Process.GetProcessById(ProcessId);
                    process.Kill();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error killing process {ProcessId}.", ProcessId);
            }
        }
    }

    public void RegisterClientStreamer(IClientStreamerConfiguration streamerConfiguration)
    {
        try
        {
            clientStreamerIds.TryAdd(streamerConfiguration.ClientId, streamerConfiguration.ClientId);
            CircularRingBuffer.RegisterClient(streamerConfiguration);

            logger.LogInformation("RegisterClientStreamer for Client ID {ClientId} to Video Stream Id {videoStreamId} {name} {RingBuffer.Id}", streamerConfiguration.ClientId, VideoStreamId, VideoStreamName, CircularRingBuffer.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering stream configuration for client {ClientId} {name} {RingBuffer.Id}", streamerConfiguration.ClientId, VideoStreamName, CircularRingBuffer.Id);
        }
    }


    public bool UnRegisterClientStreamer(Guid ClientId)
    {
        try
        {
            logger.LogInformation("UnRegisterClientStreamer ClientId: {ClientId} {name} {RingBuffer.Id}", ClientId, VideoStreamName, CircularRingBuffer.Id);
            bool result = clientStreamerIds.TryRemove(ClientId, out _);
            CircularRingBuffer.UnRegisterClient(ClientId);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error unregistering stream configuration for client {ClientId} {name} {RingBuffer.Id}", ClientId, VideoStreamName, CircularRingBuffer.Id);
            return false;
        }
    }

    private string? CheckProcessExists()
    {
        try
        {
            Process process = Process.GetProcessById(ProcessId);
            // logger.LogInformation($"Process with ID {processId} exists. Name: {process.ProcessName}");
            return process.ProcessName;
        }
        catch (ArgumentException)
        {
            // logger.LogWarning($"Process with ID {processId} does not exist.");
            return null;
        }
    }


    public IEnumerable<Guid> GetClientStreamerClientIds()
    {
        return clientStreamerIds.Keys;
    }


    public void SetFailed()
    {
        IsFailed = true;
    }

    public bool HasClient(Guid clientId)
    {
        return clientStreamerIds.ContainsKey(clientId);
    }
}