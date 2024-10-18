using StreamMaster.Domain.Dto;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IImageDownloadQueue
{
    void EnqueueNameLogo(NameLogo nameLogo);

    void EnqueueProgramMetadata(ProgramMetadata metadata);

    void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection);

    List<NameLogo> GetNextNameLogoBatch(int batchSize);

    List<ProgramMetadata> GetNextProgramMetadataBatch(int batchSize);

    bool IsNameLogoQueueEmpty();

    bool IsProgramMetadataQueueEmpty();

    int NameLogoCount { get; }

    int ProgramMetadataCount { get; }

    void TryDequeueNameLogoBatch(IEnumerable<string> names);

    void TryDequeueProgramMetadataBatch(IEnumerable<string> ids);
}