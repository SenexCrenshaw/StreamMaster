using StreamMaster.Domain.Dto;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IImageDownloadQueue
{
    void TryDequeueLogo(string name);
    void TryDequeueProgramArtwork(string id);
    void EnqueueLogo(LogoInfo logoInfo);

    void EnqueueProgramArtwork(ProgramArtwork metadata);
    void EnqueueProgramArtworkCollection(IEnumerable<ProgramArtwork> metadataCollection);

    List<LogoInfo> GetNextLogoBatch(int batchSize);

    List<ProgramArtwork> GetNextProgramArtworkBatch(int batchSize);

    bool IslogoInfoQueueEmpty();

    bool IsProgramArtworkQueueEmpty();

    int LogoCount { get; }

    int ProgramLogoCount { get; }

    void TryDequeueLogoBatch(IEnumerable<string> names);

    void TryDequeueProgramArtworkBatch(IEnumerable<string> ids);
}