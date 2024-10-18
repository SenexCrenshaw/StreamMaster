namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoService
    {
        Task<StreamResult> GetStreamAsync(int? streamGroupId, int? streamGroupProfileId, int? smChannelId, CancellationToken cancellationToken);
    }
}