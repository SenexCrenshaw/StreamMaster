namespace StreamMaster.Domain.Services
{
    public interface IVideoStreamService
    {
        void RemoveVideoStreamDto(int smChannelId);
        Task<SMStreamDto?> GetSMStreamDtoAsync(string smStreamId, CancellationToken cancellationToken);
    }
}