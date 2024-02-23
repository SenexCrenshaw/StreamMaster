using StreamMaster.Domain.Dto;

namespace StreamMaster.Domain.Services
{
    public interface IVideoStreamService
    {
        void RemoveVideoStreamDto(string videoStreamId);
        Task<VideoStreamDto?> GetVideoStreamDtoAsync(string videoStreamId, CancellationToken cancellationToken);
    }
}