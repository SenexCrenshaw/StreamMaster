using System.Text.Json;

namespace StreamMasterDomain.Repository;

public class M3UFile : AutoUpdateEntity
{
    public void WriteJSON()
    {
        string txtName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Path.GetFileNameWithoutExtension(Source) + ".json");
        string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(txtName, jsonString);
    }

    public M3UFile()
    {
        FileExtension = FileDefinitions.M3U.FileExtension;
        SMFileType = FileDefinitions.M3U.SMFileType;
    }

    public int MaxStreamCount { get; set; }
    public int StartingChannelNumber { get; set; }
    public int StationCount { get; set; }

    public DateTime LastWrite()
    {
        string fileName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source);
        DateTime lastWrite = File.GetLastWriteTime(fileName);

        return lastWrite;
    }

    public Task<List<VideoStream>?> GetM3U()
    {
        using Stream dataStream = FileUtil.GetFileDataStream(Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source));
        return Task.FromResult(IPTVExtensions.ConvertToVideoStream(dataStream, Id, Name));
    }

    public static M3UFile? ReadJSON(FileInfo fileInfo)
    {
        string filePath = Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.FullName) + ".json");

        if (!File.Exists(filePath))
        {
            return null;
        }
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<M3UFile>(jsonString);
    }
}
