using StreamMaster.SchedulesDirectAPI.Domain.JsonClasses;
using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMasterDomain.Services
{
    public interface IImageDownloadService
    {
        void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection);
        void EnqueueProgramMetadata(ProgramMetadata metadata);
        ImageDownloadServiceStatus GetStatus();
    }
}