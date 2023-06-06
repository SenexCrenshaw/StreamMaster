public class M3u8Playlist
{
    public bool IndependentSegments { get; set; }
    public int MediaSequence { get; set; }
    public string PlaylistType { get; set; }
    public List<M3u8Stream> Streams { get; set; }
    public int TargetDuration { get; set; }
    public string Uri { get; set; }
    public int Version { get; set; }
}
