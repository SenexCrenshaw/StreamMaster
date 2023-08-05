using StreamMasterDomain.Attributes;
using StreamMasterDomain.Mappings;
using StreamMasterDomain.Repository;

using System.ComponentModel.DataAnnotations;

namespace StreamMasterDomain.Dto;

[RequireAll]
public class ChannelGroupArg 
{
    public bool? IsHidden { get; set; } = false;
    public bool? IsReadOnly { get; set; } = false;

    [Required]
    [SortBy]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int Rank { get; set; }

    public string? RegexMatch { get; set; } = string.Empty;
}

[RequireAll]
public class ChannelGroupDto : ChannelGroupArg, IMapFrom<ChannelGroup>
{

    [Required]
    public int Id { get; set; }
}
