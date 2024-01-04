using StreamMaster.SchedulesDirect.Domain.Enums;
using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IEPGCache<T>
    {
        MxfGuideImage? GetGuideImageAndUpdateCache(List<ProgramArtwork>? artwork, ImageType type, string? cacheKey = null);
        Task WriteToCacheAsync<T>(string name, T data, CancellationToken cancellationToken = default);
        Task<T?> GetValidCachedDataAsync<T>(string name, CancellationToken cancellationToken = default);
        Dictionary<string, EPGJsonCache> JsonFiles { get; set; }
        void AddAsset(string md5, string? json);
        string? GetAsset(string md5);
        void ResetCache();
        void UpdateAssetImages(string md5, string? json);
        void UpdateAssetJsonEntry(string md5, string? json);
        void SaveCache();
        void ReleaseCache();
    }
}