namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IMultiViewPlayListStream
    {
        Task<(Stream? stream, int processId, ProxyStreamError? error)> HandleStream(IChannelBroadcaster channelBroadcaster, CancellationToken cancellationToken);
    }
}