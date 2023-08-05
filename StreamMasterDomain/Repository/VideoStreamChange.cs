using StreamMasterDomain.Dto;

namespace StreamMasterDomain.Repository;

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
    /// Gets or sets the type of streaming proxy to use.
    /// </summary>
    /// <value>The type of streaming proxy to use.</value>
    public StreamingProxyTypes? StreamProxyType { get; set; }
}
