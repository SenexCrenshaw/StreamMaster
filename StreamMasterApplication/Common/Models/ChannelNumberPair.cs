using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.Common.Models;

[RequireAll]
public class ChannelNumberPair
{
    public int ChannelNumber { get; set; }
    public int Id { get; set; }
}
