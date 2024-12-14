namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGFilePreviewDto
{
    public string Id { get; set; } = string.Empty;
    public string ChannelLogo { get; set; } = string.Empty;

    //public string ChannelNumber { get; set; } = string.Empty;
    public string ChannelName { get; set; } = string.Empty;
}