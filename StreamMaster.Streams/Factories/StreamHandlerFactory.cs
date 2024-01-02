using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Streams.Streams;

namespace StreamMaster.Streams.Factories;

public sealed class StreamHandlerFactory(ICircularRingBufferFactory circularRingBufferFactory, IMemoryCache memoryCache, ILogger<StreamHandler> streamHandlerLogger, IProxyFactory proxyFactory) : IStreamHandlerFactory
{
    public async Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(videoStreamDto.User_Url, videoStreamDto.User_Tvg_name, videoStreamDto.StreamingProxyType, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(videoStreamDto, ChannelId, ChannelName, rank);

        StreamHandler streamHandler = new(videoStreamDto, processId, memoryCache, streamHandlerLogger, ringBuffer);

        _ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream), cancellationToken);

        return streamHandler;
    }
}
