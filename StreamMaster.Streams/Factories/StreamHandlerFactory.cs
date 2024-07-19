using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Streams;
namespace StreamMaster.Streams.Factories;

public sealed class StreamHandlerFactory(IStreamStreamingStatisticsManager streamStreamingStatisticsManager, IOptionsMonitor<Setting> intSettings, ILoggerFactory loggerFactory, IProxyFactory proxyFactory)
    : IStreamHandlerFactory
{
    public async Task<IStreamHandler?> CreateStreamHandlerAsync(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(channelStatus, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(channelStatus, processId, intSettings, loggerFactory, streamStreamingStatisticsManager);

        _ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream), cancellationToken);

        return streamHandler;
    }
}
