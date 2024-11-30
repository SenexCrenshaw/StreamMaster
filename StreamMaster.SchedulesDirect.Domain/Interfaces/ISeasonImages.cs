namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISeasonImages : IEPGCached
    {
        Task<bool> ProcessArtAsync();
    }
}