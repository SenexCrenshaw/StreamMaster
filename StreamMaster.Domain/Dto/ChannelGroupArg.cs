using StreamMaster.Domain.Attributes;

using System.ComponentModel.DataAnnotations;

namespace StreamMaster.Domain.Dto;

[RequireAll]
public class ChannelGroupArg : ChannelGroupStreamCount
{
    public bool IsHidden { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;

    [Required]
    [SortBy]
    public string Name { get; set; } = string.Empty;

}
