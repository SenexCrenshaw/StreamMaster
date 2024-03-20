namespace StreamMaster.Domain.Dto;

public class SMStreamDto : SMStream, IMapFrom<SMStream>
{
    public int Rank { get; set; }
    public string RealUrl { get; set; }
}
