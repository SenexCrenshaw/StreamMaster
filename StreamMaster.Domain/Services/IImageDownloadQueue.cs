using StreamMaster.SchedulesDirect.Domain.JsonClasses;

namespace StreamMaster.Domain.Services;

public interface IImageDownloadQueue
{
    void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection);
    void EnqueueProgramMetadata(ProgramMetadata metadata);
    void TryDequeue(string Id);
    int Count();
    bool IsEmpty();
    ProgramMetadata? GetNext();
}
