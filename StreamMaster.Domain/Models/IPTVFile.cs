using StreamMaster.Domain.Extensions;
using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.Domain.Models;

public abstract class IPTVFile : BaseEntity
{
    private string name = string.Empty;

    public IPTVFile()
    {
        LastUpdated = SMDT.UtcNow;
        LastModified = LastUpdated;
    }

    public IPTVFile(string fileSource)
    {
        FileSource = fileSource;
        FileName = fileSource;
        LastUpdated = SMDT.UtcNow;
        LastModified = LastUpdated;
    }

    public string Description { get; set; } = string.Empty;
    public abstract string DirectoryLocation { get; }
    public int DownloadErrors { get; set; }
    public abstract string Extension { get; }
    public string FileName { get; set; } = string.Empty;
    public string FileSource { get; set; } = string.Empty;

    public DateTime LastModified { get; set; }
    public DateTime LastUpdated { get; set; }

    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(name))
            {
                name = Utils.RandomString(12);
            }
            return name;
        }

        set => name = value;
    }

    public abstract SMFileTypes SMFileType { get; }
}
