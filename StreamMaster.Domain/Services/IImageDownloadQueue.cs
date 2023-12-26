using StreamMaster.SchedulesDirect.Domain.JsonClasses;

namespace StreamMaster.Domain.Services;

public interface IImageDownloadQueue
{
    void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection);
    void EnqueueProgramMetadata(ProgramMetadata metadata);
    bool TryDequeue(out ProgramMetadata metadata);
    int Count();
    bool IsEmpty();
}
