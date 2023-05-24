using AutoMapper.Configuration.Annotations;

namespace StreamMasterDomain.Enums;

public class FileDefinition
{
    public string DirectoryLocation { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;

    [Ignore]
    public string RandomFileName { get; set; } = string.Empty;

    public SMFileTypes SMFileType { get; set; }
}
