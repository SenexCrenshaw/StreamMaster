using StreamMaster.Domain.Dto;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

using System.Collections.Concurrent;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadQueue : IImageDownloadQueue
    {
        private readonly ConcurrentDictionary<string, ProgramMetadata> programMetadataQueue = new();
        private readonly ConcurrentDictionary<string, NameLogo> nameLogoQueue = new();
        private readonly object programMetadataLock = new();
        private readonly object nameLogoLock = new();

        public void EnqueueProgramMetadata(ProgramMetadata metadata)
        {
            programMetadataQueue.TryAdd(metadata.ProgramId, metadata);
        }

        public void EnqueueNameLogo(NameLogo nameLogo)
        {
            nameLogoQueue.TryAdd(nameLogo.Name, nameLogo);
        }

        public void EnqueueProgramMetadataCollection(IEnumerable<ProgramMetadata> metadataCollection)
        {
            foreach (ProgramMetadata metadata in metadataCollection)
            {
                programMetadataQueue.TryAdd(metadata.ProgramId, metadata);
            }
        }

        public List<ProgramMetadata> GetNextProgramMetadataBatch(int batchSize)
        {
            lock (programMetadataLock)
            {
                return programMetadataQueue.Take(batchSize).Select(x => x.Value).ToList();
            }
        }

        public List<NameLogo> GetNextNameLogoBatch(int batchSize)
        {
            lock (nameLogoLock)
            {
                return nameLogoQueue.Take(batchSize).Select(x => x.Value).ToList();
            }
        }

        public void TryDequeueProgramMetadataBatch(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                programMetadataQueue.TryRemove(id, out _);
            }
        }

        public void TryDequeueNameLogoBatch(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                nameLogoQueue.TryRemove(name, out _);
            }
        }

        public int ProgramMetadataCount => programMetadataQueue.Count;
        public int NameLogoCount => nameLogoQueue.Count;

        public bool IsProgramMetadataQueueEmpty()
        {
            return programMetadataQueue.IsEmpty;
        }

        public bool IsNameLogoQueueEmpty()
        {
            return nameLogoQueue.IsEmpty;
        }
    }
}