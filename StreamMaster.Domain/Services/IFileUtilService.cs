using StreamMaster.Domain.XmltvXml;

namespace StreamMaster.Domain.Services
{
    public interface IFileUtilService
    {
        //Task<List<XmltvProgramme>> GetProgrammesFromXmlAsync(string epgPath);
        Task<List<StationChannelName>?> ProcessStationChannelNamesAsync(EPGFile epgFile, bool? ignoreCache = false);
        Task<List<StationChannelName>?> ProcessStationChannelNamesAsync(string epgPath, int epgNumber, bool? ignoreCache = false);
        Task<(int channelCount, int programCount)> ReadXmlCountsFromFileAsync(string filepath, int epgNumber);
        Task<(int channelCount, int programCount)> ReadXmlCountsFromFileAsync(EPGFile epgFile);
        Task<bool> IsXmlFileValid(string filepath);

        Task<XMLTV?> ReadXmlFileAsync(EPGFile epgFile);
        Task<XMLTV?> ReadXmlFileAsync(string filepath);
        string? GetFilePath(string filepath);
        IEnumerable<FileInfo> GetFilesFromDirectory(FileDefinition fileDefinition);
        Task<Stream?> GetFileDataStream(string source);
        bool IsFileGzipped(string filePath);
        bool IsFileZipped(string filePath);
        Task<(bool success, Exception? ex)> DownloadUrlAsync(string url, string fullName, bool? ignoreCompression = false);
        Task<(bool success, Exception? ex)> SaveFormFileAsync(dynamic data, string fileName);
        string CheckNeedsCompression(string fullName);
        string? GetExistingFilePath(string source);
        void CleanUpFile(string fullName);
    }
}