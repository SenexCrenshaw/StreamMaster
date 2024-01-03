
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Logging;

using System.Text.Json;

namespace StreamMaster.Domain.Models;

public class M3UFile : AutoUpdateEntity
{
    public void WriteJSON()
    {
        string jsonPath = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Path.GetFileNameWithoutExtension(Source) + ".json");
        string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, jsonString);
    }

    public M3UFile()
    {
        FileExtension = FileDefinitions.M3U.FileExtension;
        SMFileType = FileDefinitions.M3U.SMFileType;
    }

    public List<string> VODTags { get; set; } = [];

    //  public string OverwriteChannelNumbers { get; set; }
    public bool OverwriteChannelNumbers { get; set; } = true;
    public int MaxStreamCount { get; set; }
    public int StartingChannelNumber { get; set; }
    public int StationCount { get; set; }
    //public M3UFileStreamURLPrefix StreamURLPrefix { get; set; }
    public DateTime LastWrite()
    {
        string fileName = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source);
        DateTime lastWrite = File.GetLastWriteTime(fileName);

        return lastWrite;
    }


    [LogExecutionTimeAspect]
    public async Task<List<VideoStream>?> GetM3U(ILogger logger, CancellationToken cancellationToken)
    {
        using Stream dataStream = FileUtil.GetFileDataStream(Path.Combine(FileDefinitions.M3U.DirectoryLocation, Source));
        logger.LogInformation("Reading m3ufile {Name} and ignoring urls with {vods}", Name, string.Join(',', VODTags));
        List<VideoStream>? ret = await IPTVExtensions.ConvertToVideoStreamAsync(dataStream, Id, Name, VODTags, logger, cancellationToken);
        return ret;
    }

    public M3UFile? ReadJSON()
    {
        string jsonPath = Path.Combine(FileDefinitions.M3U.DirectoryLocation, Path.GetFileNameWithoutExtension(Source) + ".json");

        if (!File.Exists(jsonPath))
        {
            return null;
        }
        string jsonString = File.ReadAllText(jsonPath);
        return JsonSerializer.Deserialize<M3UFile>(jsonString);

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