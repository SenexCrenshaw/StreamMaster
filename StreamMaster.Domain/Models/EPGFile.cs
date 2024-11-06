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
        string jsonPath = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Source + ".json");
        string jsonString = JsonSerializer.Serialize(this, jsonSerializerOptions);
        File.WriteAllText(jsonPath, jsonString);
    }

    public EPGFile()
    {
        DirectoryLocation = FileDefinitions.EPG.DirectoryLocation;
        FileExtension = FileDefinitions.EPG.DefaultExtension;
        SMFileType = FileDefinitions.EPG.SMFileType;
    }

    public int ChannelCount { get; set; }
    public int ProgrammeCount { get; set; }
    public int TimeShift { get; set; } = 0;
}
