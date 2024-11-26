using StreamMaster.Domain.Dto;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces;

public interface IImageDownloadQueue
{
    void TryDequeuelogoInfo(string name);
    void TryDequeueProgramArtwork(string id);
    void EnqueueLogoInfo(LogoInfo  logoInfo);

    //void EnqueueProgramArtwork(ProgramArtwork metadata);

    void EnqueueProgramArtworkCollection(IEnumerable<ProgramArtwork> metadataCollection);

    List<LogoInfo > GetNextlogoInfoBatch(int batchSize);

    List<ProgramArtwork> GetNextProgramArtworkBatch(int batchSize);

    bool IslogoInfoQueueEmpty();

    bool IsProgramArtworkQueueEmpty();

    int logoInfoCount { get; }

    int ProgramArtworkCount { get; }

    void TryDequeuelogoInfoBatch(IEnumerable<string> names);

    void TryDequeueProgramArtworkBatch(IEnumerable<string> ids);
}