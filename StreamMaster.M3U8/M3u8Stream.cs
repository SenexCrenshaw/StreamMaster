public class M3u8Stream
{
    public string Audio { get; set; }
    public int AverageBandwidth { get; set; }
    public int Bandwidth { get; set; }
    public string ClosedCaptions { get; set; }
    public string Codecs { get; set; }
    public int FrameRate { get; set; }
    public string Resolution { get; set; }
    public List<M3u8Segment> Segments { get; set; }
    public string Subtitles { get; set; }
    public string Uri { get; set; }
}
