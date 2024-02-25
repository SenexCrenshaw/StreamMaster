using StreamMaster.Domain.Common;
using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.Domain.Enums;

public static class FileDefinitions
{
    public static FileDefinition ChannelIcon => new()
    {
        DirectoryLocation = BuildInfo.ChannelIconDataFolder,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.ChannelIcon
    };

    public static FileDefinition EPG => new()
    {
        DirectoryLocation = BuildInfo.EPGFolder,
        FileExtension = ".xml|.xmltv",
        SMFileType = SMFileTypes.EPG
    };

    public static FileDefinition Icon => new()
    {
        DirectoryLocation = BuildInfo.IconDataFolder,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.Icon
    };

    public static FileDefinition M3U => new()
    {
        DirectoryLocation = BuildInfo.M3UFolder,
        FileExtension = ".m3u",
        SMFileType = SMFileTypes.M3U
    };

    public static FileDefinition ProgrammeIcon => new()
    {
        DirectoryLocation = BuildInfo.ProgrammeIconDataFolder,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.ProgrammeIcon
    };

    public static FileDefinition TVLogo => new()
    {
        DirectoryLocation = BuildInfo.TVLogoDataFolder,
        FileExtension = ".jpg|.png|.jpeg",
        SMFileType = SMFileTypes.TvLogo
    };

    public static FileDefinition SDImage => new()
    {
        DirectoryLocation = BuildInfo.SDImagesFolder,
        FileExtension = ".png",
        SMFileType = SMFileTypes.SDImage
    };

    public static FileDefinition SDStationLogos => new()
    {
        DirectoryLocation = BuildInfo.SDStationLogosFolder,
        FileExtension = ".png",
        SMFileType = SMFileTypes.SDStationLogo
    };
}
