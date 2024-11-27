using System.Collections.Concurrent;

using StreamMaster.Domain.Dto;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;

namespace StreamMaster.Infrastructure.Services.Downloads
{
    public class ImageDownloadQueue() : IImageDownloadQueue
    {
        private readonly ConcurrentDictionary<string, ProgramArtwork> ProgramArtworkQueue = new();
        private readonly ConcurrentDictionary<string, LogoInfo> logoInfoQueue = new();

        //public void EnqueueProgramArtwork(ProgramArtwork metadata)
        //{
        //    _ = ProgramArtworkQueue.TryAdd(metadata.Uri, metadata);
        //}

        public void EnqueueLogoInfo(LogoInfo logoInfo)
        {
            _ = logoInfoQueue.TryAdd(logoInfo.Name, logoInfo);
        }

        public void EnqueueProgramArtworkCollection(IEnumerable<ProgramArtwork> metadataCollection)
        {
            foreach (ProgramArtwork metadata in metadataCollection)
            {
                _ = ProgramArtworkQueue.TryAdd(metadata.Uri, metadata);
            }
        }

        public List<ProgramArtwork> GetNextProgramArtworkBatch(int batchSize)
        {
            return ProgramArtworkQueue.Take(batchSize).Select(x => x.Value).ToList();
        }

        public List<LogoInfo> GetNextlogoInfoBatch(int batchSize)
        {
            return logoInfoQueue.Take(batchSize).Select(x => x.Value).ToList();
        }

        public void TryDequeueProgramArtworkBatch(IEnumerable<string> ids)
        {
            foreach (string id in ids)
            {
                _ = ProgramArtworkQueue.TryRemove(id, out _);
            }
        }

        public void TryDequeueProgramArtwork(string id)
        {
            _ = ProgramArtworkQueue.TryRemove(id, out _);
        }

        public void TryDequeuelogoInfo(string name)
        {
            _ = logoInfoQueue.TryRemove(name, out _);
        }

        public void TryDequeuelogoInfoBatch(IEnumerable<string> names)
        {
            foreach (string name in names)
            {
                _ = logoInfoQueue.TryRemove(name, out _);
            }
        }

        public int ProgramArtworkCount => ProgramArtworkQueue.Count;
        public int LogoInfoCount => logoInfoQueue.Count;

        public bool IsProgramArtworkQueueEmpty()
        {
            return ProgramArtworkQueue.IsEmpty;
        }

        public bool IslogoInfoQueueEmpty()
        {
            return logoInfoQueue.IsEmpty;
        }
    }
}