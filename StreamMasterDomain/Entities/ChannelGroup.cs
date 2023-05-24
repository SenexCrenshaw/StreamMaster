namespace StreamMasterDomain.Entities;

public class ChannelGroup : BaseEntity
{
    public new int Id { get; set; }
    public bool IsHidden { get; set; } = false;
    public bool IsReadOnly { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Rank { get; set; }
    public string RegexMatch { get; set; } = string.Empty;
}
