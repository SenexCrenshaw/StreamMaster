namespace StreamMaster.Domain.Dto;

public class SMStreamDto : SMStream, IMapFrom<SMStream>
{
    public string RealUrl { get; set; }
}
