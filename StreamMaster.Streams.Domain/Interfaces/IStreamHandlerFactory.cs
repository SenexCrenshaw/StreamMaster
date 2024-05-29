using StreamMaster.Domain.Models;

namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IStreamHandlerFactory
    {
        Task<IStreamHandler?> CreateStreamHandlerAsync(SMChannel smChannel, SMStream smStream, int rank, CancellationToken cancellationToken);
        //Task<IStreamHandler?> RestartStreamHandlerAsync(IStreamHandler StreamHandler);
    }
}