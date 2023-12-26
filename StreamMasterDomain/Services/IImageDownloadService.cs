

using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMasterDomain.Services
{
    public interface IImageDownloadService
    {
        void Start();
        ImageDownloadServiceStatus GetStatus();
    }
}