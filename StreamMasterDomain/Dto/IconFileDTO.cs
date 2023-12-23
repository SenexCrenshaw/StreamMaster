using StreamMasterDomain.Attributes;
using StreamMasterDomain.Models;

using System.Text.Json.Serialization;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class IconFileDto : IMapFrom<IconFile>
{
    [JsonIgnore]
    public string Extension { get; set; } = string.Empty;

    [JsonIgnore]
    public int FileId { get; set; }

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public SMFileTypes SMFileType { get; set; }

    public string Source { get; set; } = string.Empty;
}
