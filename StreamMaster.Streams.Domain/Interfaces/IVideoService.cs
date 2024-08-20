namespace StreamMaster.Streams.Domain.Interfaces
{
    public interface IVideoService
    {
        Task<(Stream? stream, IClientConfiguration? clientConfiguration, string? Redirect)> GetStreamAsync(int? streamGroupId, int? streamGroupProfileId, int? smChannelId, CancellationToken cancellationToken);
    }
}