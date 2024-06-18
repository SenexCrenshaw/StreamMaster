using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Streams;
namespace StreamMaster.Streams.Factories;

public sealed class StreamHandlerFactory(IInputStatisticsManager inputStatisticsManager, IOptionsMonitor<Setting> intsettings, ILoggerFactory loggerFactory, IProxyFactory proxyFactory)
    : IStreamHandlerFactory
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<IStreamHandler?> CreateStreamHandlerAsync(IChannelStatus channelStatus, CancellationToken cancellationToken)
    {
        SMStream smStream = channelStatus.SMStream;
        SMChannel smChannel = channelStatus.SMChannel;
        VideoOutputProfileDto videoProfile = channelStatus.VideoProfile;
        int rank = channelStatus.Rank;

        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(smStream.Url, smStream.Name, videoProfile, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(channelStatus, processId, intsettings, loggerFactory, inputStatisticsManager);

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
