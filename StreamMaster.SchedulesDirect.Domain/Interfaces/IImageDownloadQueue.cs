using StreamMaster.Domain.Dto;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IImageDownloadQueue
{
    void EnqueueNameLogo(NameLogo nameLogo);

    void EnqueueProgramArtwork(ProgramArtwork metadata);

    void EnqueueProgramArtworkCollection(IEnumerable<ProgramArtwork> metadataCollection);

    List<NameLogo> GetNextNameLogoBatch(int batchSize);

    List<ProgramArtwork> GetNextProgramArtworkBatch(int batchSize);

    bool IsNameLogoQueueEmpty();

    bool IsProgramArtworkQueueEmpty();

    int NameLogoCount { get; }

    int ProgramArtworkCount { get; }

    void TryDequeueNameLogoBatch(IEnumerable<string> names);

    void TryDequeueProgramArtworkBatch(IEnumerable<string> ids);
}