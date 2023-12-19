using Microsoft.Extensions.Logging;

using StreamMasterApplication.Common.Interfaces;

using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;

using StreamMasterInfrastructure.VideoStreamManager.Streams;

namespace StreamMasterInfrastructure.VideoStreamManager.Factories;

public sealed class StreamHandlerFactory(ICircularRingBufferFactory circularRingBufferFactory, ILogger<StreamHandler> streamHandlerlogger, IProxyFactory proxyFactory) : IStreamHandlerFactory
{
    public async Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelName, int rank, CancellationToken cancellationToken)
    {
        (Stream? stream, int processId, ProxyStreamError? error) = await proxyFactory.GetProxy(videoStreamDto.User_Url, videoStreamDto.User_Tvg_name, cancellationToken).ConfigureAwait(false);
        if (stream == null || error != null || processId == 0)
        {
            return null;
        }

        ICircularRingBuffer ringBuffer = circularRingBufferFactory.CreateCircularRingBuffer(videoStreamDto, ChannelName, rank);

        StreamHandler streamHandler = new(videoStreamDto, processId, streamHandlerlogger, ringBuffer);

        //_ = streamHandler.StartVideoStreamingAsync(stream, ringBuffer).ConfigureAwait(false);

        _ = Task.Run(() => streamHandler.StartVideoStreamingAsync(stream, ringBuffer), cancellationToken);

        return streamHandler;
    }
}
