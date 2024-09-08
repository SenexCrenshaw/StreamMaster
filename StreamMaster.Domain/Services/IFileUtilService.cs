namespace StreamMaster.Domain.Services
{
    public interface IFileUtilService
    {
        IEnumerable<FileInfo> GetFilesFromDirectory(FileDefinition fileDefinition);
        Stream? GetFileDataStream(string source);
        bool IsFileGzipped(string filePath);
        bool IsFileZipped(string filePath);
        Task<(bool success, Exception? ex)> DownloadUrlAsync(string url, string fullName, bool? ignoreCompression = false);
        Task<(bool success, Exception? ex)> SaveFormFileAsync(dynamic data, string fileName);
        string CheckNeedsCompression(string fullName);
        string? GetExistingFilePath(string source);
    }
}