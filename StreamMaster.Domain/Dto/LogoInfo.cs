using StreamMaster.Domain.Crypto;

namespace StreamMaster.Domain.Dto;

/// <summary>
/// Represents information about a logo, including its URL, file type, and related metadata.
/// </summary>
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class LogoInfo
{
    public LogoInfo() { }
    /// <summary>
    /// Initializes a new instance of <see cref="LogoInfo"/> with the specified URL and optional parameters.
    /// </summary>
    /// <param name="url">The URL of the logo.</param>
    /// <param name="iconType">The file type of the logo.</param>
    /// <param name="isSchedulesDirect">Indicates if the logo comes from Schedules Direct.</param>
    public LogoInfo(string url, SMFileTypes iconType = SMFileTypes.Logo, bool isSchedulesDirect = false)
    {
        string cleanedUrl = Cleanup(url);
        IsSVG = cleanedUrl.EndsWithIgnoreCase(".svg");
        string ext = Path.GetExtension(cleanedUrl);
        // Use .png for filename and extension if SVG
        Ext = IsSVG ? ".png" : string.IsNullOrEmpty(ext) ? ".png" : ext;
        Id = cleanedUrl.GenerateFNV1aHash(withExtension: false);

        if (isSchedulesDirect && !url.StartsWithIgnoreCase("image/"))
        {
            url = "image/" + url;
        }

        Url = url;
        SMFileType = iconType;
        FullPath = FileName.GetImageFullPath(iconType) ?? string.Empty;
        IsSchedulesDirect = isSchedulesDirect;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LogoInfo"/> with a name and URL.
    /// </summary>
    /// <param name="name">The name of the logo.</param>
    /// <param name="url">The URL of the logo.</param>
    /// <param name="iconType">The file type of the logo.</param>
    /// <param name="isSchedulesDirect">Indicates if the logo comes from Schedules Direct.</param>
    public LogoInfo(string name, string url, SMFileTypes iconType = SMFileTypes.Logo, bool isSchedulesDirect = false)
        : this(url, iconType, isSchedulesDirect)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="LogoInfo"/> from an <see cref="SMStream"/>.
    /// </summary>
    /// <param name="smStream">The stream containing logo information.</param>
    public LogoInfo(SMStream smStream)
        : this(
            smStream.Name,
            smStream.Logo,
            smStream.SMStreamType switch
            {
                SMStreamTypeEnum.CustomPlayList => SMFileTypes.CustomPlayListLogo,
                _ => SMFileTypes.Logo
            })
    {
        IsSchedulesDirect = false;
    }

    /// <summary>
    /// Gets the file extension of the logo, defaulting to .png if SVG.
    /// </summary>
    public string Ext { get; } = string.Empty;

    /// <summary>
    /// Gets the unique identifier for the logo, based on the original URL.
    /// </summary>
    public string Id { get; } = string.Empty;

    /// <summary>
    /// Gets the name of the logo.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Gets the filename of the logo, including the ID and extension (always .png if SVG).
    /// </summary>
    public string FileName => $"{Id}{Ext}";

    /// <summary>
    /// Indicates whether the logo originates from Schedules Direct.
    /// </summary>
    public bool IsSchedulesDirect { get; set; }

    /// <summary>
    /// Gets the full file path of the logo.
    /// </summary>
    public string FullPath { get; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the logo.
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Indicates whether the original URL was an SVG file.
    /// </summary>
    public bool IsSVG { get; } = false;

    /// <summary>
    /// Gets the file type of the logo.
    /// </summary>
    public SMFileTypes SMFileType { get; }

    /// <summary>
    /// Cleans up a logo URL by removing unnecessary prefixes.
    /// </summary>
    /// <param name="url">The URL to clean.</param>
    /// <returns>The cleaned URL.</returns>
    public static string Cleanup(string url)
    {
        url = url.StartsWithIgnoreCase("image/") ? url[6..] : url;
        if (url.Contains('?'))
        {
            url = url[..url.IndexOf('?')];
        }

        return url;
    }
}
