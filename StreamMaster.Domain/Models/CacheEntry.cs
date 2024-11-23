using System.ComponentModel.DataAnnotations.Schema;

namespace StreamMaster.Domain.Models;

public class CacheEntity : BaseEntity
{
    public CacheEntity()
    {
        ContentType = "application/json";
        MinimumMinutesBetweenDownloads = 2;
    }

    [Column(TypeName = "citext")]
    public string ContentType { get; set; } = string.Empty;

    [NotMapped]
    public string Value { get; set; } = string.Empty;
    public int DownloadErrors { get; set; }

    public bool FileExists { get; set; }

    //[Column(TypeName = "citext")]
    [NotMapped]
    public string DirectoryLocation { get; set; } = string.Empty;

    [Column(TypeName = "citext")]
    public string FileExtension { get; set; } = string.Empty;
    public DateTime LastDownloadAttempt { get; set; }
    public DateTime LastDownloaded { get; set; }
    public DateTime LastUpdated { get; set; }
    public int MinimumMinutesBetweenDownloads { get; set; }

    [Column(TypeName = "citext")]
    public string Name { get; set; } = string.Empty;

    public SMFileTypes SMFileType { get; set; }

    [Column(TypeName = "citext")]
    public string Source { get; set; } = string.Empty;

    public void SetFileDefinition(FileDefinition fd)
    {
        if (string.IsNullOrEmpty(FileExtension))
        {
            FileExtension = fd.DefaultExtension;
        }

        SMFileType = fd.SMFileType;
    }
}
