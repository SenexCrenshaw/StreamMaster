namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ILineupService : IEPGCached, IDisposable
    {
        Task<bool> BuildLineupServices(CancellationToken cancellationToken = default);

        Task<List<SubscribedLineup>> GetLineups(CancellationToken cancellationToken);

        Task<List<StationChannelMap>> GetStationChannelMaps(CancellationToken cancellationToken);

        Task<List<StationPreview>> GetStationPreviews(CancellationToken cancellationToken);
    }
}