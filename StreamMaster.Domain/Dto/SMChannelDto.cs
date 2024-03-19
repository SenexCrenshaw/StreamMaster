namespace StreamMaster.Domain.Dto;

public class SMChannelDto : SMChannel, IMapFrom<SMChannel>
{
    public string RealUrl { get; set; }
}
