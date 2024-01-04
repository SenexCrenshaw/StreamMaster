namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IDescriptions : IEPGCached
    {
        Task<bool> BuildAllGenericSeriesInfoDescriptions();
    }
}