using StreamMaster.SchedulesDirect.Domain.JsonClasses;

using StreamMasterDomain.Services;

using System.Collections.Concurrent;

namespace StreamMasterInfrastructure.Services.Downloads;

public class ImageDownloadQueue : IImageDownloadQueue
{
    private readonly ConcurrentQueue<ProgramMetadata> downloadQueue = new();

    public void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection)
    {
        foreach (ProgramMetadata metadata in metadataCollection)
        {
            downloadQueue.Enqueue(metadata);
        }
    }

    public void EnqueueProgramMetadata(ProgramMetadata metadata)
    {
        downloadQueue.Enqueue(metadata);
    }

    public bool TryDequeue(out ProgramMetadata metadata)
    {
        return downloadQueue.TryDequeue(out metadata);
    }

    public int Count()
    {
        return downloadQueue.Count;
    }

    public bool IsEmpty()
    {
        return downloadQueue.IsEmpty;
    }
}