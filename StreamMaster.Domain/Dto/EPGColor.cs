namespace StreamMaster.Domain.Dto;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGColorDto : IMapFrom<EPGFile>
{
    public int Id { get; set; }
    public int EPGNumber { get; set; }
    public string StationId { get; set; } = string.Empty;
    public string Color { get; set; } = "#FFFFFF";
}
