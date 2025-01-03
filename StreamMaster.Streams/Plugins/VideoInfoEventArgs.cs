namespace StreamMaster.Streams.Plugins;

/// <summary>
/// Event arguments for video info updates.
/// </summary>
public class VideoInfoEventArgs(VideoInfo videoInfo, string id) : EventArgs
{
    /// <summary>
    /// The updated video information.
    /// </summary>
    public VideoInfo VideoInfo { get; } = videoInfo;

    /// <summary>
    /// The unique identifier for the video source.
    /// </summary>
    public string Id { get; } = id;
}
