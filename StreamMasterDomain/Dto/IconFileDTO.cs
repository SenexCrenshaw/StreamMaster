using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class IconFileDto : IMapFrom<IconFile>
{
    public bool FileExists { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OriginalSource { get; set; } = string.Empty;
    public SMFileTypes SMFileType { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
