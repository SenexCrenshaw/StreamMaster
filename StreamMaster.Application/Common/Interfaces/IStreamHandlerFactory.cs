using StreamMaster.Domain.Dto;

namespace StreamMaster.Application.Common.Interfaces
{
    public interface IStreamHandlerFactory
    {
        Task<IStreamHandler?> CreateStreamHandlerAsync(VideoStreamDto videoStreamDto, string ChannelId, string ChannelName, int rank, CancellationToken cancellationToken);
    }
}