using StreamMasterDomain.Dto;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Entities;

public class VideoStreamBaseUpdate
{
    [Key]
    public int Id { get; set; }

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

public class VideoStreamUpdate : VideoStreamBaseUpdate
{
    public List<ChildVideoStreamDto>? ChildVideoStreams { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video stream is currently active.
    /// </summary>
    /// <value>True if the video stream is currently active; otherwise, false.</value>
    public bool? IsActive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video stream has been marked
    /// as deleted.
    /// </summary>
    /// <value>
    /// True if the video stream has been marked as deleted; otherwise, false.
    /// </value>
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video stream should be hidden.
    /// </summary>
    /// <value>True if the video stream should be hidden; otherwise, false.</value>
    public bool? IsHidden { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video stream is read-only.
    /// </summary>
    /// <value>True if the video stream is read-only; otherwise, false.</value>
    public bool? IsReadOnly { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the video stream was created by
    /// a user.
    /// </summary>
    /// <value>True if the video stream was created by a user; otherwise, false.</value>
    public bool? IsUserCreated { get; set; }

    /// <summary>
    /// Gets or sets the number of errors encountered during streaming.
    /// </summary>
    /// <value>The number of errors encountered during streaming.</value>
    public int? StreamErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the time of the last failed streaming attempt.
    /// </summary>
    /// <value>The time of the last failed streaming attempt.</value>
    public DateTime? StreamLastFail { get; set; }

    /// <summary>
    /// Gets or sets the time of the last successful streaming attempt.
    /// </summary>
    /// <value>The time of the last successful streaming attempt.</value>
    public DateTime? StreamLastStream { get; set; }

    /// <summary>
    /// Gets or sets the type of streaming proxy to use.
    /// </summary>
    /// <value>The type of streaming proxy to use.</value>
    public StreamingProxyTypes? StreamProxyType { get; set; }
}
