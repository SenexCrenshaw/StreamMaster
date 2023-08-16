using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

public class IconSimpleDto : IMapFrom<IconFileDto>, IMapFrom<IconFile>
{
    public int Id { get; set; }
    public string Source { get; set; }
    public string Name { get; set; }
}
