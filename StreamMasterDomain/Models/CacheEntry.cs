namespace StreamMasterDomain.Models;

public class CacheEntity : BaseEntity
{
    public CacheEntity()
    {
        ContentType = "application/json";
        MinimumMinutesBetweenDownloads = 2;
    }

    public string ContentType { get; set; } = string.Empty;
    public int DownloadErrors { get; set; }

    public bool FileExists { get; set; }
    public string FileExtension { get; set; } = string.Empty;
    public DateTime LastDownloadAttempt { get; set; }
    public DateTime LastDownloaded { get; set; }
    public DateTime LastUpdated { get; set; }
    public int MinimumMinutesBetweenDownloads { get; set; }

    public string Name { get; set; } = string.Empty;

    public SMFileTypes SMFileType { get; set; }

    public string Source { get; set; } = string.Empty;

    public void SetFileDefinition(FileDefinition fd)
    {
        if (string.IsNullOrEmpty(FileExtension))
        {
            FileExtension = fd.FileExtension;
        }

        SMFileType = fd.SMFileType;
    }
}
