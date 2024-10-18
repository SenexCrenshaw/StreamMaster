namespace StreamMaster.Streams.Domain.Interfaces;

public interface IStreamFactory
{
    //Task<Stream> CreateStreamAsync(string streamUrl);
    Task<(Stream? stream, int processId, ProxyStreamError? error)> GetStream(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);
}