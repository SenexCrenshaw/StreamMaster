namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISportsImages : IEPGCached
    {
        List<MxfProgram> SportEvents { get; set; }
        Task<bool> GetAllSportsImages();
    }
}