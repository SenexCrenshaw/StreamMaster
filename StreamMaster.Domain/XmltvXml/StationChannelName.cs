namespace StreamMaster.Domain.XmltvXml;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationChannelName
{
    public string Id { get; set; }
    public string Channel { get; set; }
    public string ChannelName { get; set; }
    public string DisplayName { get; set; }
    public string Logo { get; set; }
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
