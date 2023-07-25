using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class IconFileDto : IMapFrom<IconFile>
{
    public string ContentType { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
}
