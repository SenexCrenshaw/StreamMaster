using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.Domain.Services
{
    public interface IImageDownloadService
    {
        void Start();
        ImageDownloadServiceStatus GetStatus();
    }
}