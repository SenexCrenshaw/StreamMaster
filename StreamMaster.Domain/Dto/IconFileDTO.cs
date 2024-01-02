using StreamMaster.Domain.Attributes;
using StreamMaster.Domain.Models;

using System.Text.Json.Serialization;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class IconFileDto : IMapFrom<IconFile>
{
    [JsonIgnore]
    public string Extension { get; set; } = string.Empty;

    [JsonIgnore]
    public int FileId { get; set; }

    public string Id => Source;

    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public SMFileTypes SMFileType { get; set; }

    public string Source { get; set; } = string.Empty;
}
