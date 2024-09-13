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

        public ProgramMetadata? GetNextProgramMetadata()
        {
            return programMetadataQueue.IsEmpty ? null : programMetadataQueue.First().Value;
        }

        public NameLogo? GetNextNameLogo()
        {
            return nameLogoQueue.IsEmpty ? null : nameLogoQueue.First().Value;
        }

        public void TryDequeueProgramMetadata(string id)
        {
            programMetadataQueue.TryRemove(id, out _);
        }

        public void TryDequeueNameLogo(string id)
        {
            nameLogoQueue.TryRemove(id, out _);
        }

        public int ProgramMetadataCount()
        {
            return programMetadataQueue.Count;
        }

        public int NameLogoCount()
        {
            return nameLogoQueue.Count;
        }

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
