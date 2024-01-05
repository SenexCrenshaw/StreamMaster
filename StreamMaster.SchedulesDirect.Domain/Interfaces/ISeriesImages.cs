using System.Collections.Specialized;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISeriesImages : IEPGCached
    {
        NameValueCollection SportsSeries { get; set; }
        Task<bool> GetAllSeriesImages();
    }
}