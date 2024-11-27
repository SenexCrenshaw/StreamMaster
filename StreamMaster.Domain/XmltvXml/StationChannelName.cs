namespace StreamMaster.Domain.XmltvXml;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationChannelName
{
    public string Id { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string ChannelName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Logo { get; set; } = string.Empty;
    public int EPGNumber { get; set; }

    private StationChannelName() { }
    public StationChannelName(string channel, string displayName, string channelName, string logo, int epgNumber)
    {
        Id = $"{epgNumber}-{channel}";
        Channel = channel;
        ChannelName = channelName;
        DisplayName = displayName;
        Logo = logo;
        EPGNumber = epgNumber;
    }
}
