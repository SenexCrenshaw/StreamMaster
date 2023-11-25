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

    private async Task<StreamHandler?> CreateStreamHandler(VideoStreamDto videoStreamDto, int rank, CancellationToken cancellation)
    {
        CancellationTokenSource cancellationTokenSource = new();

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(videoStreamDto, rank);

        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(videoStreamDto.User_Url, videoStreamDto.User_Tvg_name, cancellation);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(videoStreamDto.User_Url, videoStreamDto.Id, videoStreamDto.User_Tvg_name, processId, streamHandlerlogger, ringBuffer, clientStreamerManager, cancellationTokenSource);

        _ = streamHandler.StartVideoStreamingAsync(stream, ringBuffer);

        return streamHandler;
    }

    public IStreamHandler? GetStreamHandler(string videoStreamId)
    {
        ICollection<string> videoStreamIds = _streamHandlers.Keys;
        foreach (IStreamHandler handler in _streamHandlers.Values)
        {
            if (handler.ClientCount > 1)
            {
                logger.LogInformation("GetStreamHandler StreamHandler: {videoStreamId} has more than one client: {count}", videoStreamId, handler.ClientCount);
            }
        }
        if (!_streamHandlers.TryGetValue(videoStreamId, out IStreamHandler? streamHandler))
        {
            return null;
        }

        return streamHandler;
    }

    public async Task<IStreamHandler?> GetOrCreateStreamHandler(VideoStreamDto videoStreamDto, int rank, CancellationToken cancellation = default)
    {
        _streamHandlers.TryGetValue(videoStreamDto.Id, out IStreamHandler? streamHandler);

        if (streamHandler?.IsFailed != false)
        {
            logger.LogInformation("Creating new handler for stream: {Id} {name}", videoStreamDto.Id, videoStreamDto.User_Tvg_name);
            streamHandler = await CreateStreamHandler(videoStreamDto, rank, cancellation);
            if (streamHandler == null)
            {
                return null;
            }
            _streamHandlers.TryAdd(videoStreamDto.Id, streamHandler);
            return streamHandler;
        }

        logger.LogInformation("Reusing handler for stream: {Id} {name}", videoStreamDto.Id, videoStreamDto.User_Tvg_name);
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

    public void MoveClientStreamers(IEnumerable<Guid> cliendIds, IStreamHandler newStreamHandler)
    {
        clientStreamerManager.MoveClientStreamers(cliendIds, newStreamHandler);
        //if (oldStreamHandler.ClientCount == 0)
        //{
        StopAndUnRegisterHandler(newStreamHandler.VideoStreamId);
        //}
    }

    public IStreamHandler? GetStreamHandlerByClientId(Guid ClientId)
    {
        List<IStreamHandler> handlers = GetStreamHandlers();
        foreach (IStreamHandler handler in handlers)
        {
            ICollection<IClientStreamerConfiguration>? test = handler.GetClientStreamerConfigurations();
        }
        return _streamHandlers.Values.FirstOrDefault(x => x.HasClient(ClientId));
    }

    public bool StopAndUnRegisterHandler(string VideoStreamId)
    {
        if (_streamHandlers.TryRemove(VideoStreamId, out IStreamHandler? streamHandler))
        {
            streamHandler.Stop();
            return true;
        }

        //logger.LogWarning("Failed to remove stream information for {VideoStreamId}", VideoStreamId);
        return false;
    }
}