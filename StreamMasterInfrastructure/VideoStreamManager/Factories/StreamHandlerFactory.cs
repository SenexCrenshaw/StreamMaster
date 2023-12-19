using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.VideoStreamManager.Streams;

namespace StreamMasterInfrastructure.VideoStreamManager.Factories;

public sealed class StreamHandlerFactory(ICircularRingBufferFactory circularRingBufferFactory, IMemoryCache memoryCache, ILogger<StreamHandler> streamHandlerlogger, IProxyFactory proxyFactory) : IStreamHandlerFactory
{
    public async Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(videoStreamDto.User_Url, videoStreamDto.User_Tvg_name, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(videoStreamDto, ChannelId, ChannelName, rank);

        StreamHandler streamHandler = new(videoStreamDto, processId, memoryCache, streamHandlerlogger, ringBuffer);

        //_ = streamHandler.StartVideoStreamingAsync(stream, ringBuffer).ConfigureAwait(false);

        _ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream), cancellationToken);

        return streamHandler;
    }
}
