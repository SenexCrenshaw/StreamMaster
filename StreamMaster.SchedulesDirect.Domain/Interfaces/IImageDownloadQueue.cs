
using StreamMaster.Domain.Dto;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IImageDownloadQueue
{
    void EnqueueNameLogo(NameLogo nameLogo);
    void EnqueueProgramMetadata(ProgramMetadata metadata);
    void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection);
    NameLogo? GetNextNameLogo();
    ProgramMetadata? GetNextProgramMetadata();
    bool IsNameLogoQueueEmpty();
    bool IsProgramMetadataQueueEmpty();
    int NameLogoCount();
    int ProgramMetadataCount();
    void TryDequeueNameLogo(string id);
    void TryDequeueProgramMetadata(string id);
}