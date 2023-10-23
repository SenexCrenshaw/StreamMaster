using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager.Streams;

public class StreamManager(
    ICircularRingBufferFactory circularRingBufferFactory,
    IProxyFactory proxyFactory,
    IClientStreamerManager clientStreamerManager,
    ILogger<StreamHandler> streamHandlerlogger,
    ILogger<StreamManager> logger
    ) : IStreamManager
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

    private async Task<StreamHandler?> CreateStreamHandler(ChildVideoStreamDto childVideoStreamDto, int rank, CancellationToken cancellation)
    {
        CancellationTokenSource cancellationTokenSource = new();

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(childVideoStreamDto, rank);

        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(childVideoStreamDto.User_Url, cancellation);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(childVideoStreamDto.User_Url, childVideoStreamDto.Id, processId, streamHandlerlogger, ringBuffer, clientStreamerManager, cancellationTokenSource);

        _ = streamHandler.StartVideoStreamingAsync(stream, ringBuffer);

        return streamHandler;
    }

    public IStreamHandler? GetStreamHandler(string videoStreamId)
    {
        if (!_streamHandlers.TryGetValue(videoStreamId, out IStreamHandler? streamHandler))
        {
            return null;
        }

        return streamHandler;
    }

    public async Task<IStreamHandler?> GetOrCreateStreamHandler(ChildVideoStreamDto childVideoStreamDto, int rank, CancellationToken cancellation = default)
    {
        if (!_streamHandlers.TryGetValue(childVideoStreamDto.Id, out IStreamHandler? streamHandler))
        {
            logger.LogInformation("Creating new buffer for stream: {Id}", childVideoStreamDto.Id);
            streamHandler = await CreateStreamHandler(childVideoStreamDto, rank, cancellation);
            if (streamHandler == null)
            {
                return null;
            }
            _streamHandlers.TryAdd(childVideoStreamDto.Id, streamHandler);
            return streamHandler;
        }

        logger.LogInformation("Reusing buffer for stream: {Id}", childVideoStreamDto.Id);
        return streamHandler;
    }

    public IStreamHandler? GetStreamHandlerFromStreamUrl(string streamUrl)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.StreamUrl == streamUrl);
    }

    public int GetStreamsCountForM3UFile(int m3uFileId)
    {
        return _streamHandlers.Count(x => x.Value.M3UFileId == m3uFileId);
    }

    public List<IStreamHandler> GetStreamHandlers()
    {
        if (_streamHandlers.Values == null)
        {
            return new List<IStreamHandler>();
        }

        return _streamHandlers.Values.ToList();
    }

    public void MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler)
    {
        clientStreamerManager.MoveClientStreamers(oldStreamHandler, newStreamHandler);
        //if (oldStreamHandler.ClientCount == 0)
        //{
        StopAndUnRegisterHandler(oldStreamHandler.VideoStreamId);
        //}
    }

    public IStreamHandler? GetStreamHandlerByClientId(Guid ClientId)
    {
        return _streamHandlers.Values.FirstOrDefault(x => x.HasClient(ClientId));
    }

    public bool StopAndUnRegisterHandler(string VideoStreamId)
    {
        if (_streamHandlers.TryRemove(VideoStreamId, out IStreamHandler? streamHandler))
        {
            logger.LogWarning("Stopping stream of {VideoStreamId}", VideoStreamId);
            streamHandler.Stop();
            return true;
        }

        logger.LogWarning("Failed to remove stream information for {VideoStreamId}", VideoStreamId);
        return false;
    }
}