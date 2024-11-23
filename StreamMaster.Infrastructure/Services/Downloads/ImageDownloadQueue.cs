using System.Collections.Concurrent;

using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Dto;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadQueue(IOptionsMonitor<Setting> settings) : IImageDownloadQueue
    {
        private readonly ConcurrentDictionary<string, ProgramArtwork> ProgramArtworkQueue = new();
        private readonly ConcurrentDictionary<string, NameLogo> nameLogoQueue = new();

        public void EnqueueProgramArtwork(ProgramArtwork metadata)
        {
            _ = ProgramArtworkQueue.TryAdd(metadata.Uri, metadata);
        }

        public void EnqueueNameLogo(NameLogo nameLogo)
        {
            _ = nameLogoQueue.TryAdd(nameLogo.Name, nameLogo);
        }

        public void EnqueueProgramArtworkCollection(IEnumerable<ProgramArtwork> metadataCollection)
        {
            return;
            //foreach (ProgramArtwork metadata in metadataCollection)
            //{
            //    _ = ProgramArtworkQueue.TryAdd(metadata.Uri, metadata);
            //}
        }

        public List<ProgramArtwork> GetNextProgramArtworkBatch(int batchSize)
        {
            return ProgramArtworkQueue.Take(batchSize).Select(x => x.Value).ToList();
        }

        public List<NameLogo> GetNextNameLogoBatch(int batchSize)
        {
            return nameLogoQueue.Take(batchSize).Select(x => x.Value).ToList();
        }

        public void TryDequeueProgramArtworkBatch(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                _ = ProgramArtworkQueue.TryRemove(id, out _);
            }
        }

        public void TryDequeueNameLogoBatch(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                _ = nameLogoQueue.TryRemove(name, out _);
            }
        }

        public int ProgramArtworkCount => ProgramArtworkQueue.Count;
        public int NameLogoCount => nameLogoQueue.Count;

        public bool IsProgramArtworkQueueEmpty()
        {
            return ProgramArtworkQueue.IsEmpty;
        }

        public bool IsNameLogoQueueEmpty()
        {
            return nameLogoQueue.IsEmpty;
        }
    }
}