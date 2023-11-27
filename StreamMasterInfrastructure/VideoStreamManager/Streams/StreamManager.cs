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

    private async Task<StreamHandler?> CreateStreamHandler(VideoStreamDto videoStreamDto, string ChannelName, int rank, CancellationToken cancellation)
    {
        CancellationTokenSource cancellationTokenSource = new();

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(videoStreamDto, ChannelName, rank);

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
        return !_streamHandlers.TryGetValue(videoStreamId, out IStreamHandler? streamHandler) ? null : streamHandler;
    }

    public async Task<IStreamHandler?> GetOrCreateStreamHandler(VideoStreamDto videoStreamDto, string ChannelName, int rank, CancellationToken cancellation = default)
    {
        _ = _streamHandlers.TryGetValue(videoStreamDto.User_Url, out IStreamHandler? streamHandler);

        if (streamHandler?.IsFailed == true)
        {
            _ = StopAndUnRegisterHandler(videoStreamDto.User_Url);
            _ = _streamHandlers.TryGetValue(videoStreamDto.User_Url, out streamHandler);
        }

        if (streamHandler?.IsFailed != false)
        {
            logger.LogInformation("Creating new handler for stream: {Id} {name}", videoStreamDto.Id, videoStreamDto.User_Tvg_name);
            streamHandler = await CreateStreamHandler(videoStreamDto, ChannelName, rank, cancellation);
            if (streamHandler == null)
            {
                return null;
            }
            _ = _streamHandlers.TryAdd(videoStreamDto.User_Url, streamHandler);
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
        return _streamHandlers.Values == null ? ([]) : ([.. _streamHandlers.Values]);
    }

    public void MoveClientStreamers(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler)
    {
        clientStreamerManager.MoveClientStreamers(oldStreamHandler, newStreamHandler);
        if (oldStreamHandler.ClientCount == 0)
        {
            _ = StopAndUnRegisterHandler(oldStreamHandler.VideoStreamId);
        }
    }

    public IStreamHandler? GetStreamHandlerByClientId(Guid ClientId)
    {
        //List<IStreamHandler> handlers = GetStreamHandlers();
        //foreach (IStreamHandler handler in handlers)
        //{
        //    ICollection<IClientStreamerConfiguration>? test = handler.GetClientStreamerConfigurations();
        //}
        return _streamHandlers.Values.FirstOrDefault(x => x.HasClient(ClientId));
    }

    public bool StopAndUnRegisterHandler(string VideoStreamUrl)
    {
        if (_streamHandlers.TryRemove(VideoStreamUrl, out IStreamHandler? streamHandler))
        {
            streamHandler.Stop();
            return true;
        }

        //logger.LogWarning("Failed to remove stream information for {VideoStreamId}", VideoStreamId);
        return false;
    }
}