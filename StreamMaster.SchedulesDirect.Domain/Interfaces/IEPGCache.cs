using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IEPGCache<T>
    {
        List<string> GetExpiredKeys();
        void RemovedExpiredKeys(List<string>? keysToDelete = null);
        void UpdateProgramArtworkCache(List<ProgramArtwork> artwork, ImageType type, string? cacheKey = null);
        Task WriteToCacheAsync(string name, T data, CancellationToken cancellationToken = default);
        Task<T?> GetValidCachedDataAsync(string name, CancellationToken cancellationToken = default);
        Dictionary<string, EPGJsonCache> JsonFiles { get; set; }
        void AddAsset(string md5, string? json);
        string? GetAsset(string md5);
        void ResetCache();
        void AddOrUpdateAsset(string md5, string? json);
        void SaveCache();
        void ReleaseCache();
    }
}