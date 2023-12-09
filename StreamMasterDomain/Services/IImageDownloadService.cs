using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterDomain.Services
{
    public interface IImageDownloadService
    {
        ImageDownloadServiceStatus GetStatus();
    }
}