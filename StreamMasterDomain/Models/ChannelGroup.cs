namespace StreamMasterDomain.Models;

public class ChannelGroup : BaseEntity
{

    public new int Id { get; set; }
    public bool IsHidden { get; set; } = false;
    public bool IsReadOnly { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegexMatch { get; set; } = string.Empty;

}
