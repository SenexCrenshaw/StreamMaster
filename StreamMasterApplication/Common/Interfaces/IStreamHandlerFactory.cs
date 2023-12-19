namespace StreamMasterApplication.Common.Interfaces
{
    public interface IStreamHandlerFactory
    {
        Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelName, int rank, CancellationToken cancellationToken);
    }
}