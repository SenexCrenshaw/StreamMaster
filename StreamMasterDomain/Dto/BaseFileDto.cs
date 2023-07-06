using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class BaseFileDto
{
    public bool AutoUpdate { get; set; }
    public int HoursToUpdate { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DownloadErrors { get; set; }
    public int Id { get; set; }
    public DateTime LastDownloadAttempt { get; set; }
    public DateTime LastDownloaded { get; set; }
    public string MetaData { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool NeedsUpdate { get; set; }
    public string OriginalSource { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
