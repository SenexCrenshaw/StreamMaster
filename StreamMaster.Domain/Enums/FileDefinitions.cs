using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Enums;

public static class FileDefinitions
{
    private static FileDefinition CreateDefinition(string folder, string fileExtensions, SMFileTypes type)
    {
        return new FileDefinition
        {
            DefaultExtension = fileExtensions.Split("|")[0],
            DirectoryLocation = folder,
            FileExtensions = fileExtensions,
            SMFileType = type
        };
    }

    //public static FileDefinition ChannelLogo { get; } = CreateDefinition(BuildInfo.ChannelLogoDataFolder, ".jpg|.png|.jpeg", SMFileTypes.ChannelLogo);
    public static FileDefinition EPG { get; } = CreateDefinition(BuildInfo.EPGFolder, ".xml|.xmltv", SMFileTypes.EPG);
    public static FileDefinition Logo { get; } = CreateDefinition(BuildInfo.LogoFolder, ".jpg|.png|.jpeg", SMFileTypes.Logo);
    public static FileDefinition M3U { get; } = CreateDefinition(BuildInfo.M3UFolder, ".m3u", SMFileTypes.M3U);
    public static FileDefinition TVLogo { get; } = CreateDefinition(BuildInfo.TVLogoFolder, ".jpg|.png|.jpeg", SMFileTypes.TvLogo);
    public static FileDefinition SDImage { get; } = CreateDefinition(BuildInfo.SDImagesFolder, ".png", SMFileTypes.SDImage);
    public static FileDefinition SDStationLogos { get; } = CreateDefinition(BuildInfo.SDStationLogosFolder, ".png", SMFileTypes.SDStationLogo);

    public static FileDefinition? GetFileDefinition(SMFileTypes fileType)
    {
        return fileType switch
        {
            SMFileTypes.EPG => EPG,
            SMFileTypes.Logo => Logo,
            SMFileTypes.M3U => M3U,
            SMFileTypes.TvLogo => TVLogo,
            SMFileTypes.SDImage => SDImage,
            SMFileTypes.SDStationLogo => SDStationLogos,
            _ => null
        };
    }
}