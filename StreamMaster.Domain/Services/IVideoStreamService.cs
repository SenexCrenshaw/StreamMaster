namespace StreamMaster.Domain.Services
{
    public interface IVideoStreamService
    {
        void RemoveVideoStreamDto(string smStreamId);
        Task<SMStreamDto?> GetSMStreamDtoAsync(string smStreamId, CancellationToken cancellationToken);
    }
}