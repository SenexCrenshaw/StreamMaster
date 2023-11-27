using StreamMasterDomain.Models;

namespace StreamMasterDomain.Dto;

public class ChannelLogoDto : IconFile
{
    public string EPGId { get; set; }
    public int EPGFileId { get; set; }

}
