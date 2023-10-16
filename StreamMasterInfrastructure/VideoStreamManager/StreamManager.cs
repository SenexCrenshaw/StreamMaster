using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class StreamManager(ICircularRingBufferFactory circularRingBufferFactory, IProxyFactory proxyFactory, ILogger<StreamHandler> streamHandlerlogger, ILogger<StreamManager> logger, IMemoryCache memoryCache) : IStreamManager
{
    private readonly ConcurrentDictionary<string, IStreamHandler> _streamHandlers = new();

    public void Dispose()
    {
        // Dispose of each stream controller
        foreach (IStreamHandler streamController in _streamHandlers.Values)
        {
            streamController.Dispose();
        }

        // Clear the dictionary
        _streamHandlers.Clear();
    }

    private async Task<StreamHandler?> CreateStreamHandler(ChildVideoStreamDto childVideoStreamDto, int rank)
    {
        CancellationTokenSource cancellationTokenSource = new();

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(childVideoStreamDto, rank);

        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(childVideoStreamDto.User_Url);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(childVideoStreamDto, processId, streamHandlerlogger, ringBuffer, cancellationTokenSource);

        _ = streamHandler.StartVideoStreamingAsync(stream, ringBuffer);

        return streamHandler;
    }

    public async Task<IStreamHandler?> GetOrCreateStreamHandler(ChildVideoStreamDto childVideoStreamDto, int rank)
    {
        if (!_streamHandlers.TryGetValue(childVideoStreamDto.User_Url, out IStreamHandler? streamHandler))
        {
            streamHandler = await CreateStreamHandler(childVideoStreamDto, rank);
            if (streamHandler == null)
            {
                return null;
            }
            _streamHandlers.TryAdd(childVideoStreamDto.User_Url, streamHandler);
        }
        else
        {
            logger.LogInformation("Reusing buffer for stream: {StreamUrl}", childVideoStreamDto.User_Url);
        }

        return streamHandler;
    }

    public IStreamHandler? GetStreamInformationFromStreamUrl(string streamUrl)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }

    public ICollection<IStreamHandler> GetStreamInformations()
    {
        return _streamHandlers.Values;
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamHandlers.Count(x => x.Value.M3UFileId == m3uFileId);
    }
    public IStreamHandler? Stop(string streamUrl)
    {
        if (_streamHandlers.TryRemove(streamUrl, out IStreamHandler? streamInformation))
        {
            streamInformation.Stop();
            return streamInformation;
        }
        logger.LogWarning("Failed to remove stream information for {StreamUrl}", streamUrl);
        return null;
    }
}
