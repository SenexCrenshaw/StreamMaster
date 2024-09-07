namespace StreamMaster.Domain.Enums;

public class FileDefinition
{
    public string DefaultExtension { get; set; } = string.Empty;
    public string DirectoryLocation { get; set; } = string.Empty;
    public string FileExtensions { get; set; } = string.Empty;
    public SMFileTypes SMFileType { get; set; }
}
