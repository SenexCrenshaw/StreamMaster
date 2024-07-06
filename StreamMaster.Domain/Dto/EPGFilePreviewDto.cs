namespace StreamMaster.Domain.Dto;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGFilePreviewDto
{
    public string Id { get; set; }
    public string ChannelLogo { get; set; }
    public string ChannelNumber { get; set; }
    public string ChannelName { get; set; }
}
