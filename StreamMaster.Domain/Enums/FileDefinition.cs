using AutoMapper.Configuration.Annotations;

using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.Domain.Enums;

public class FileDefinition
{
    public string DirectoryLocation { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;

    [Ignore]
    public string RandomFileName { get; set; } = string.Empty;

    public SMFileTypes SMFileType { get; set; }
}
