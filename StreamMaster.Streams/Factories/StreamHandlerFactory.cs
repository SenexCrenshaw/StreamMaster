using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Cache;
using StreamMaster.Streams.Streams;
namespace StreamMaster.Streams.Factories;

public sealed class StreamHandlerFactory(IInputStatisticsManager inputStatisticsManager, IMemoryCache memoryCache, ILoggerFactory loggerFactory, IProxyFactory proxyFactory) : IStreamHandlerFactory
{
    public async Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(videoStreamDto.User_Url, videoStreamDto.User_Tvg_name, videoStreamDto.StreamingProxyType, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(videoStreamDto, processId, ChannelId, ChannelName, rank, memoryCache, loggerFactory, inputStatisticsManager);

        _ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream), cancellationToken);

        return streamHandler;
    }

    public async Task<IStreamHandler?> RestartStreamHandlerAsync(IStreamHandler StreamHandler)
    {
        Setting setting = memoryCache.GetSetting();

        if (StreamHandler.RestartCount > setting.MaxStreamReStart)
        {
            return null;
        }

        StreamHandler.RestartCount++;

        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(StreamHandler.VideoStreamDto.User_Url, StreamHandler.VideoStreamDto.User_Tvg_name, StreamHandler.VideoStreamDto.StreamingProxyType, CancellationToken.None).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        _ = Task.Run(() => StreamHandler.StartVideoStreamingAsync(stream));

        return StreamHandler;
    }
}
