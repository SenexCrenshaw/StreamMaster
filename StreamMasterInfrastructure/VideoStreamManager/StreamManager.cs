using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;
using StreamMasterApplication.Common.Models;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.VideoStreamManager.Buffers;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.VideoStreamManager;

public class StreamManager(ICircularRingBufferFactory circularRingBufferFactory, IChannelService channelService, ILogger<RingBufferReadStream> ringBufferReadStreamLogger, IProxyFactory proxyFactory, ILogger<StreamHandler> streamHandlerlogger, ILogger<StreamManager> logger) : IStreamManager
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

        StreamHandler streamHandler = new(childVideoStreamDto.User_Url, childVideoStreamDto.Id, processId, streamHandlerlogger, ringBuffer, cancellationTokenSource);

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

    public IStreamHandler? Stop(string videoStreamId)
    {
        if (_streamHandlers.TryRemove(videoStreamId, out IStreamHandler? streamHandler))
        {
            logger.LogWarning("Stopping stream {videoStreamId}", videoStreamId);
            streamHandler.Stop();
            return streamHandler;
        }
        logger.LogWarning("Failed to remove stream information for {videoStreamId}", videoStreamId);
        return null;
    }

    public void MoveClientStreamer(IStreamHandler oldStreamHandler, IStreamHandler newStreamHandler)// string OldStreamUrl, string NewStreamUrl, ClientStreamerConfiguration config)
    {

        ICollection<ClientStreamerConfiguration>? oldConfigs = oldStreamHandler.GetClientStreamerConfigurations();

        if (oldConfigs != null)
        {
            foreach (ClientStreamerConfiguration oldConfig in oldConfigs)
            {

                oldStreamHandler.UnRegisterClientStreamer(oldConfig);
                newStreamHandler.RegisterClientStreamer(oldConfig);
            }

            if (oldStreamHandler.ClientCount == 0)
            {
                Stop(oldStreamHandler.VideoStreamId);
            }
        }
    }
}
