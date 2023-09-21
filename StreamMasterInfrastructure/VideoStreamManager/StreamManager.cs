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

public class StreamManager(ILogger<CircularRingBuffer> circularBufferLogger, ILogger<StreamManager> logger, IMemoryCache memoryCache) : IStreamManager
{
    //private readonly ILogger<CircularRingBuffer> _circularBufferLogger = loggerFactory.CreateLogger<CircularRingBuffer>();
    //private readonly ILogger<StreamManager> logger = loggerFactory.CreateLogger<StreamManager>();
    private readonly ConcurrentDictionary<string, IStreamInformation> _streamInformations = new();

    public void Dispose()
    {
        _streamInformations.Clear();
    }

    public async Task<IStreamInformation?> GetOrCreateBuffer(ChildVideoStreamDto childVideoStreamDto, string videoStreamId, string videoStreamName, int rank)
    {
        string streamUrl = childVideoStreamDto.User_Url;
        if (_streamInformations.TryGetValue(streamUrl, out IStreamInformation? _streamInformation))
        {
            logger.LogInformation("Reusing buffer for stream: {StreamUrl}", streamUrl);
            return _streamInformation;
        }

        logger.LogInformation("Creating and starting buffer for stream: {StreamUrl}", streamUrl);

        Setting setting = memoryCache.GetSetting();

        ICircularRingBuffer buffer = new CircularRingBuffer(childVideoStreamDto, videoStreamId, videoStreamName, rank, setting.PreloadPercentage, setting.RingBufferSizeMB, circularBufferLogger);
        CancellationTokenSource cancellationTokenSource = new();

        (Stream? stream, int processId, ProxyStreamError? error) = await GetProxy(streamUrl, cancellationTokenSource.Token);

        if (stream == null || error != null)
        {
            logger.LogError("Error GetOrCreateBuffer for {StreamUrl}", streamUrl);
            return null;
        }

        try
        {

            Task streamingTask = StartVideoStreaming(stream, streamUrl, buffer, cancellationTokenSource);

            StreamInformation streamStreamInfo = new(streamUrl, buffer, streamingTask, childVideoStreamDto.M3UFileId, childVideoStreamDto.MaxStreams, processId, cancellationTokenSource);

            _streamInformations.TryAdd(streamStreamInfo.StreamUrl, streamStreamInfo);

            logger.LogInformation("Buffer created and streaming started for: {StreamUrl}", streamUrl);

            return streamStreamInfo;
        }
        catch (TaskCanceledException)
        {
            throw ;
        }
        return null;
    }

    public SingleStreamStatisticsResult GetSingleStreamStatisticsResult(string streamUrl)
    {
        if (_streamInformations.TryGetValue(streamUrl, out IStreamInformation? _streamInformation))
        {
            return _streamInformation.RingBuffer.GetSingleStreamStatisticsResult();
        }
        return new SingleStreamStatisticsResult();
    }

    public IStreamInformation? GetStreamInformationFromStreamUrl(string streamUrl)
    {
        return _streamInformations.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }

    public ICollection<IStreamInformation> GetStreamInformations()
    {
        return _streamInformations.Values;
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamInformations
            .Count(x => x.Value.M3UFileId == m3uFileId);
    }

    public IStreamInformation? Stop(string streamUrl)
    {
        if (_streamInformations.TryRemove(streamUrl, out IStreamInformation? _streamInformation))
        {
            _streamInformation.Stop();
            return _streamInformation;
        }

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
