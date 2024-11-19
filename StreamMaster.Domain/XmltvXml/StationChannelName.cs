namespace StreamMaster.Domain.XmltvXml;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationChannelName(string channel, string displayName, string channelName, int epgNumber)
{
    public string Id { get; set; } = $"{epgNumber}-{channel}";
    public string Channel { get; set; } = channel;
    public string ChannelName { get; set; } = channelName;
    public string DisplayName { get; set; } = displayName;
    public int EPGNumber { get; set; } = epgNumber;
}
