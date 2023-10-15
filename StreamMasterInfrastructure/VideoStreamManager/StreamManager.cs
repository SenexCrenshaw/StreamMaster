using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Enums;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class StreamManager(ILogger<CircularRingBuffer> circularBufferLogger, IClientStreamerManager clientStreamerManager, ILogger<StreamManager> logger, IMemoryCache memoryCache) : IStreamManager
{

    private readonly ConcurrentDictionary<string, IStreamController> _streamControllers = new();

    public void Dispose()
    {
        // Dispose of each stream controller
        foreach (IStreamController streamController in _streamControllers.Values)
        {
            streamController.Dispose();
        }

        // Clear the dictionary
        _streamControllers.Clear();
    }
    private async Task<IStreamController> CreateNewStreamController(string streamUrl, ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank)
    {
        Setting setting = memoryCache.GetSetting();

        ICircularRingBuffer buffer = new CircularRingBuffer(childVideoStreamDto, videoStreamId, videoStreamName, rank, setting.PreloadPercentage, setting.RingBufferSizeMB, circularBufferLogger);
        CancellationTokenSource cancellationTokenSource = new();

        (Stream? stream, int processId, ProxyStreamError? error) = await GetProxy(streamUrl, cancellationTokenSource.Token);

        if (stream == null || error != null)
        {
            logger.LogError("Error in CreateNewStreamController for {StreamUrl}", streamUrl);
            throw new InvalidOperationException($"Unable to create StreamController for {streamUrl} due to proxy error.");
        }

        Task streamingTask = StartVideoStreaming(stream, streamUrl, buffer, cancellationTokenSource);

        StreamController streamController = new(streamUrl, clientStreamerManager, buffer, streamingTask, childVideoStreamDto.M3UFileId, childVideoStreamDto.MaxStreams, processId, cancellationTokenSource);

        logger.LogInformation("Created new StreamController for: {StreamUrl}", streamUrl);

        return streamController;
    }


    public async Task<IStreamController?> GetOrCreateStreamController(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank)
    {
        string streamUrl = childVideoStreamDto.User_Url;


        if (!_streamControllers.TryGetValue(streamUrl, out IStreamController streamController))
        {
            // If the stream controller doesn't exist, create a new one
            IStreamController newStreamController = await CreateNewStreamController(streamUrl, childVideoStreamDto, videoStreamId, videoStreamName, rank);

            // Add or get existing StreamController
            streamController = _streamControllers.AddOrUpdate(streamUrl, newStreamController, (key, existing) => existing);

            if (streamController.StreamUrl == streamUrl)
            {
                logger.LogInformation("Created buffer for stream: {StreamUrl}", streamUrl);
            }
        }
        else
        {
            logger.LogInformation("Reusing buffer for stream: {StreamUrl}", streamUrl);
        }

        return streamController;
    }


    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl)
    {
        if (_streamControllers.TryGetValue(streamUrl, out IStreamController? _streamInformation))
        {
            return _streamInformation.RingBuffer.GetSingleStreamStatisticsResult();
        }
        return new SingleStreamStatisticsResult();
    }

    public IStreamController? GetStreamInformationFromStreamUrl(string streamUrl)
    {
        return _streamControllers.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }

    public ICollection<IStreamController> GetStreamInformations()
    {
        return _streamControllers.Values;
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamControllers.Count(x => x.Value.M3UFileId == m3uFileId);
    }
    public IStreamController? Stop(string streamUrl)
    {
        if (_streamControllers.TryRemove(streamUrl, out IStreamController? streamInformation))
        {
            streamInformation.Stop();
            return streamInformation;
        }
        logger.LogWarning("Failed to remove stream information for {StreamUrl}", streamUrl);
        return null;
    }
    private async Task DelayWithCancellation(int milliseconds, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(milliseconds, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Task was cancelled");
            throw;
        }
    }
    private async Task<(Stream? stream, int processId, ProxyStreamError? error)> GetProxy(string streamUrl, CancellationToken cancellationToken)
    {
        Stream? stream;
        ProxyStreamError? error;
        int processId;

        Setting setting = memoryCache.GetSetting();

        if (setting.StreamingProxyType == StreamingProxyTypes.FFMpeg)
        {
            (stream, processId, error) = await StreamingProxies.GetFFMpegStream(streamUrl, logger, setting);
            LogErrorIfAny(stream, error, streamUrl);
        }
        else
        {
            (stream, processId, error) = await StreamingProxies.GetProxyStream(streamUrl, logger, setting, cancellationToken);
            LogErrorIfAny(stream, error, streamUrl);
        }

        return (stream, processId, error);
    }

    private void LogBufferHealth(ICircularRingBuffer buffer)
    {
        // Calculate buffer utilization percentage
        float bufferUtilization = buffer.GetBufferUtilization();
        if (bufferUtilization < 95.0)
        {
            // Log the buffer health information
            logger.LogWarning("Buffer health below 95% - Capacity: {BufferCapacity}, Utilization: {BufferUtilization}%", buffer.BufferSize, bufferUtilization);
        }
    }

    private void LogErrorIfAny(Stream? stream, ProxyStreamError? error, string streamUrl)
    {
        if (stream == null || error != null)
        {
            logger.LogError("Error getting proxy stream for {StreamUrl}: {Error?.Message}", streamUrl, error?.Message);
        }
    }

    private async Task LogRetryAndDelay(int retryCount, int maxRetries, int waitTime, string streamUrl, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            logger.LogInformation("Stream was cancelled for: {StreamUrl}", streamUrl);
        }

        logger.LogInformation("Stream received 0 bytes for stream: {StreamUrl} Retry {retryCount}/{maxRetries}",
            streamUrl,
            retryCount,
            maxRetries);

        await DelayWithCancellation(waitTime, token);
    }

    private async Task StartVideoStreaming(Stream stream, string streamUrl, ICircularRingBuffer buffer, CancellationTokenSource cancellationToken)
    {
        int chunkSize = 24 * 1024;

        logger.LogInformation("Starting video read streaming, chunk size is {ChunkSize}, for stream: {StreamUrl}", chunkSize, streamUrl);

        byte[] bufferChunk = new byte[chunkSize];

        int maxRetries = 3; //setting.MaxConnectRetry > 0 ? setting.MaxConnectRetry : 3;
        int waitTime = 50;// setting.MaxConnectRetryTimeMS > 0 ? setting.MaxConnectRetryTimeMS : 50;

        using (stream)
        {
            int retryCount = 0;
            while (!cancellationToken.IsCancellationRequested && retryCount < maxRetries)
            {
                try
                {
                    int bytesRead = await TryReadStream(bufferChunk, stream, cancellationToken.Token);
                    if (bytesRead == -1)
                    {
                        break;
                    }
                    if (bytesRead == 0)
                    {
                        retryCount++;
                        await LogRetryAndDelay(retryCount, maxRetries, waitTime, streamUrl, cancellationToken.Token);
                    }
                    else
                    {
                        buffer.WriteChunk(bufferChunk, bytesRead);
                        retryCount = 0;
                        //LogBufferHealth(buffer);
                    }
                }
                catch (TaskCanceledException ex)
                {
                    logger.LogError(ex, "Stream cancelled for: {StreamUrl}", streamUrl);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Stream error for: {StreamUrl}", streamUrl);
                    break;
                }
            }
        }

        logger.LogInformation("Stream stopped for: {StreamUrl}", streamUrl);
        if (!cancellationToken.IsCancellationRequested)
        {
            cancellationToken.Cancel();
        }
    }

    private async Task<int> TryReadStream(byte[] bufferChunk, Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            if (!stream.CanRead || cancellationToken.IsCancellationRequested)
            {
                logger.LogWarning("Stream is not readable or cancelled");
                return -1;
            }

            return await stream.ReadAsync(bufferChunk, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            return -1;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
