namespace StreamMasterDomain.Entities;

public class AutoUpdateEntity : CacheEntity
{
    public AutoUpdateEntity()
    {
        HoursToUpdate = 72;
        AutoUpdate = true;
    }

    public bool AutoUpdate { get; set; }
    public int HoursToUpdate { get; set; }
    public string Description { get; set; } = string.Empty;
}

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
    public string MetaData { get; set; } = string.Empty;
    public int MinimumMinutesBetweenDownloads { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OriginalSource { get; set; } = string.Empty;
    public SMFileTypes SMFileType { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Url { get; set; }

    public void SetFileDefinition(FileDefinition fd)
    {
        if (string.IsNullOrEmpty(FileExtension))
        {
            FileExtension = fd.FileExtension;
        }

        SMFileType = fd.SMFileType;
    }
}