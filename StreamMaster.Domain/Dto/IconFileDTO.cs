using StreamMaster.Domain.Enums;
using StreamMaster.Domain.Mappings;
using StreamMaster.Domain.Models;

using StreamMaster.Domain.Attributes;

using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Dto;

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
