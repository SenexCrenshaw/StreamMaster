
namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IImageDownloadQueue
{
    void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection);
    void EnqueueProgramMetadata(ProgramMetadata metadata);
    void TryDequeue(string Id);
    int Count();
    bool IsEmpty();
    ProgramMetadata? GetNext();
}
