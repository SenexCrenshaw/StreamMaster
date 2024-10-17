namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISportsImages : IEPGCached, IDisposable
    {
        List<MxfProgram> SportEvents { get; set; }

        Task<bool> GetAllSportsImages();
    }
}