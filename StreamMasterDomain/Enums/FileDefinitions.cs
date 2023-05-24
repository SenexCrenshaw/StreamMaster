namespace StreamMasterDomain.Enums;

public static class FileDefinitions
{
    public static FileDefinition EPG => new()
    {
        DirectoryLocation = Constants.EPGDirectory,
        FileExtension = Constants.EPGExtension,
        SMFileType = SMFileTypes.EPG
    };

    public static FileDefinition Icon => new()
    {
        DirectoryLocation = Constants.IconDataDirectory,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.Icon
    };

    public static FileDefinition M3U => new()
    {
        DirectoryLocation = Constants.M3UDirectory,
        FileExtension = Constants.M3UExtension,
        SMFileType = SMFileTypes.M3U
    };

    public static FileDefinition ProgrammeIcon => new()
    {
        DirectoryLocation = Constants.ProgrammeIconDataDirectory,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.ProgrammeIcon
    };

    public static FileDefinition TVLogo => new()
    {
        DirectoryLocation = Constants.TVLogoDirectory,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.TvLogo
    };
}
