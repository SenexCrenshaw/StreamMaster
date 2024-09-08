using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace StreamMaster.Domain.Models;

public class EPGFile : AutoUpdateEntity
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };
    public static string APIName => "EPGFiles";
    public int EPGNumber { get; set; }

    [Column(TypeName = "citext")]
    public string Color { get; set; } = "FFFFFF";

    public static EPGFile? ReadJSON(FileInfo fileInfo)
    {
        if (string.IsNullOrEmpty(fileInfo.DirectoryName))
        {
            return null;
        }
        string filePath = Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.FullName) + ".json");

        if (!File.Exists(filePath))
        {
            return null;
        }
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<EPGFile>(jsonString);
    }

    public void WriteJSON()
    {
        string jsonPath = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(Source) + ".json");
        string jsonString = JsonSerializer.Serialize(this, jsonSerializerOptions);
        File.WriteAllText(jsonPath, jsonString);
    }

    public EPGFile()
    {
        FileExtension = FileDefinitions.EPG.DefaultExtension;
        SMFileType = FileDefinitions.EPG.SMFileType;
    }

    public DateTime LastWrite()
    {
        string fileName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Source);
        return File.Exists(fileName)
            ? File.GetLastWriteTime(fileName)
            : File.Exists(fileName + ".gz")
            ? File.GetLastWriteTime(fileName + ".gz")
            : File.Exists(fileName + ".zip") ? File.GetLastWriteTime(fileName + ".zip") : default;
    }

    public int ChannelCount { get; set; }
    public int ProgrammeCount { get; set; }
    public int TimeShift { get; set; } = 0;
}
