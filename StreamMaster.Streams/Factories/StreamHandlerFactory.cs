using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Streams;
namespace StreamMaster.Streams.Factories;

public sealed class StreamHandlerFactory(IStreamStreamingStatisticsManager streamStreamingStatisticsManager, IOptionsMonitor<Setting> intSettings, ILoggerFactory loggerFactory, IProxyFactory proxyFactory)
    : IStreamHandlerFactory
{

    public async Task<IStreamHandler?> CreateStreamHandlerAsync(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        SMStreamDto smStream = channelStatus.SMStream;
        SMChannelDto smChannel = channelStatus.SMChannel;
        VideoOutputProfileDto videoProfile = channelStatus.VideoProfile;
        Setting settings = intSettings.CurrentValue;
        //int rank = channelStatus.CurrentRank;

        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(smStream, settings.StreamingClientUserAgent, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(channelStatus, processId, intSettings, loggerFactory, streamStreamingStatisticsManager);

        _ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream), cancellationToken);

        return streamHandler;
    }

    //public async Task<IStreamHandler?> RestartStreamHandlerAsync(IStreamHandler StreamHandler)
    //{


    //    if (StreamHandler.RestartCount > settings.MaxStreamReStart)
    //    {
    //        return null;
    //    }

    //    StreamHandler.RestartCount++;

    //    (ClientStream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(StreamHandler.SMStream.Url, StreamHandler.SMStream.Name, StreamHandler.SMStream.StreamingProxyType, CancellationToken.None).ConfigureAwait(false);
    //    if (stream == null || error != null || processId == 0)
    //    {
    //        return null;
    //    }

    //    _ = Task.Run(() => StreamHandler.StartVideoStreamingAsync(stream));

    //    return StreamHandler;
    //}
}
