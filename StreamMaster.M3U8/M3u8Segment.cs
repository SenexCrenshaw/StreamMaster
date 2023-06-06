public class M3u8Segment
{
    public long ByterangeLength { get; set; } = -1;
    public long ByterangeStart { get; set; }
    public double Duration { get; set; }
    public int MediaSequence { get; set; }
    public string Uri { get; set; }
}
