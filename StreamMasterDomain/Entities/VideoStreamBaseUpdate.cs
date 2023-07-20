using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Entities;

public class VideoStreamBaseUpdate
{
    [Key]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the channel number for the video stream.
    /// </summary>
    /// <value>The channel number for the video stream.</value>
    public int? Tvg_chno { get; set; }

    /// <summary>
    /// Gets or sets the EPG group for the video stream.
    /// </summary>
    /// <value>The EPG group for the video stream.</value>
    public string? Tvg_group { get; set; }

    /// <summary>
    /// Gets or sets the EPG ID for the video stream.
    /// </summary>
    /// <value>The EPG ID for the video stream.</value>
    public string? Tvg_ID { get; set; }

    /// <summary>
    /// Gets or sets the URL for the channel logo for the video stream.
    /// </summary>
    public string? Tvg_logo { get; set; }

    /// <summary>
    /// Gets or sets the callsign for the video stream.
    /// </summary>
    /// <value>The callsign for the video stream.</value>
    public string? Tvg_name { get; set; }

    /// <summary>
    /// Gets or sets the URL for the video stream.
    /// </summary>
    /// <value>
    /// A nullable string representing the URL for the video stream. If a URL is
    /// assigned, the value is a string. If no URL is assigned, the value is null.
    /// </value>
    public string? Url { get; set; }
}
