namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamHandlerFactory
    {
        Task<IStreamHandler?> CreateStreamHandlerAsync(IChannelStatus channelStatus, CancellationToken cancellationToken);
        //Task<IStreamHandler?> RestartStreamHandlerAsync(IStreamHandler StreamHandler);
    }
}