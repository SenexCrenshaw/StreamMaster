using StreamMaster.Domain.Configuration;
using StreamMaster.Streams.Streams;
namespace StreamMaster.Streams.Factories;

public sealed class StreamHandlerFactory(IInputStatisticsManager inputStatisticsManager, IOptionsMonitor<Setting> intsettings, ILoggerFactory loggerFactory, IProxyFactory proxyFactory)
    : IStreamHandlerFactory
{
    private readonly Setting settings = intsettings.CurrentValue;

    public async Task<IStreamHandler?> CreateStreamHandlerAsync(SMChannel smChannel, SMStream smStream, int rank, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(smStream.Url, smStream.Name, smChannel.StreamingProxyType, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        StreamHandler streamHandler = new(smStream, smChannel, processId, rank, intsettings, loggerFactory, inputStatisticsManager);

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

    //    (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(StreamHandler.SMStream.Url, StreamHandler.SMStream.Name, StreamHandler.SMStream.StreamingProxyType, CancellationToken.None).ConfigureAwait(false);
    //    if (stream == null || error != null || processId == 0)
    //    {
    //        return null;
    //    }

    //    _ = Task.Run(() => StreamHandler.StartVideoStreamingAsync(stream));

    //    return StreamHandler;
    //}
}
