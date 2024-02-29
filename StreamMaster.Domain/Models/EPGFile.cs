using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace StreamMaster.Domain.Models;

public class EPGFile : AutoUpdateEntity
{
    public int EPGNumber { get; set; }

    [Column(TypeName = "citext")]
    public string Color { get; set; }

    public static EPGFile? ReadJSON(FileInfo fileInfo)
    {
        string filePath = Path.Combine(fileInfo.DirectoryName, Path.GetFileNameWithoutExtension(fileInfo.FullName) + ".json");

        if (!File.Exists(filePath))
        {
            return null;
        }
        string jsonString = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<EPGFile>(jsonString);
    }

    public void WriteJSON(ILogger logger)
    {
        try
        {
            string txtName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Path.GetFileNameWithoutExtension(Source) + ".json");
            string jsonString = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(txtName, jsonString);
        }
        catch
(Exception ex)
        {
            logger.LogError(ex.Message);
        }
    }
    public EPGFile()
    {
        //DirectoryLocation = FileDefinitions.EPG.DirectoryLocation;
        FileExtension = FileDefinitions.EPG.FileExtension;
        SMFileType = FileDefinitions.EPG.SMFileType;
    }
    public DateTime LastWrite()
    {
        string fileName = Path.Combine(FileDefinitions.EPG.DirectoryLocation, Source);
        DateTime lastWrite = File.GetLastWriteTime(fileName);

        return lastWrite;
    }
    public int ChannelCount { get; set; }
    public int ProgrammeCount { get; set; }
    public int TimeShift { get; set; } = 0;

}
