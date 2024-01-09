namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamHandlerFactory
    {
        Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank, CancellationToken cancellationToken);
        Task<IStreamHandler?> RestartStreamHandlerAsync(IStreamHandler StreamHandler);
    }
}