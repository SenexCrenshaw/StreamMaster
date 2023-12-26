namespace StreamMaster.Domain.Dto;

public class VideoStreamIsReadOnly
{
    public int Rank { get; set; }
    public bool IsReadOnly { get; set; }
    public string VideoStreamId { get; set; }
}
