using StreamMasterDomain.Attributes;

namespace StreamMasterDomain.Dto;

public class ChannelLogoDto
{
    public string EPGId { get; set; }
    public int EPGFileId { get; set; }

    [IndexBy("logoUrl")]    
    public string LogoUrl { get; set; }
}
