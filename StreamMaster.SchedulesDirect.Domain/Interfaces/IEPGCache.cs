using StreamMaster.SchedulesDirect.Domain.Models;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IEPGCache
    {
        Dictionary<string, EPGJsonCache> JsonFiles { get; set; }
        void AddAsset(string md5, string? json);
        void CleanDictionary();
        void CloseCache();
        bool DeleteFile(string filepath);
        string? GetAsset(string md5);
        void LoadCache();
        void ResetEPGCache();
        dynamic ReadJsonFile(string filepath, Type type);
        void UpdateAssetImages(string md5, string? json);
        void UpdateAssetJsonEntry(string md5, string? json);
        void WriteCache();
        bool WriteJsonFile(object obj, string filepath);
    }
}