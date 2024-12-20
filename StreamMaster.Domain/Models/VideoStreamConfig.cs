using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Helpers;

namespace StreamMaster.Domain.Models;

public class VideoStreamConfig
{
    public int EPGNumber { get; set; }
    /// <summary>
    /// Gets or sets the unique identifier for the channel.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the channel.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the EPG identifier for the channel.
    /// </summary>
    public string EPGId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL or path to the channel's logo.
    /// </summary>
    public string Logo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the group to which the channel belongs.
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the channel number.
    /// </summary>
    public int ChannelNumber { get; set; }

    /// <summary>
    /// Gets or sets the time shift for the channel in minutes.
    /// </summary>
    public int TimeShift { get; set; }

    /// <summary>
    /// Indicates whether the channel is a duplicate of another channel.
    /// </summary>
    public bool IsDuplicate { get; set; } = false;

    /// <summary>
    /// Gets or sets the M3U file identifier associated with the channel.
    /// </summary>
    public int M3UFileId { get; set; } = 0;

    /// <summary>
    /// Gets or sets the file position in the M3U file.
    /// </summary>
    //public int FilePosition { get; set; } = 0;

    public string EncodedString { get; set; } = string.Empty;
    public string CleanName { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;

    public string GroupTitle { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the command profile associated with the channel.
    /// </summary>
    public CommandProfileDto? CommandProfile { get; set; }

    /// <summary>
    /// Gets or sets the output profile for the channel.
    /// </summary>
    public OutputProfileDto? OutputProfile { get; set; }

    public bool IsDummy => M3UFileId == EPGHelper.DummyId;
    public bool IsCustom => M3UFileId == EPGHelper.CustomPlayListId;
    public bool IsIntro => M3UFileId == EPGHelper.IntroPlayListId;

    //public bool IsShort { get; set; } = false;
    public string BaseUrl { get; set; } = string.Empty;
    public int StreamGroupProfileId { get; set; }

}