using StreamMaster.Domain.Services;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services.Downloads;

public class ImageDownloadQueue : IImageDownloadQueue
{
    private readonly ConcurrentDictionary<string, ProgramMetadata> downloadQueue = new();

    public void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection)
    {
        foreach (ProgramMetadata metadata in metadataCollection)
        {
            downloadQueue.TryAdd(metadata.ProgramId, metadata);
        }
    }

    public void EnqueueProgramMetadata(ProgramMetadata metadata)
    {
        downloadQueue.TryAdd(metadata.ProgramId, metadata);
    }

    public ProgramMetadata? GetNext()
    {
        return downloadQueue.Keys.Count == 0 ? null : downloadQueue.First().Value;
    }

    public void TryDequeue(string Id)
    {

        downloadQueue.TryRemove(Id, out _);
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