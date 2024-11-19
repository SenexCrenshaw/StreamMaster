using StreamMaster.Domain.Attributes;

namespace StreamMaster.Domain.Dto;

[RequireAll]
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class EPGFileDto : BaseFileDto, IMapFrom<EPGFile>
{
    public int TimeShift { get; set; }
    public int EPGNumber { get; set; }
    public string Color { get; set; } = "#FFFFFF";
    public int ChannelCount { get; set; }
    public DateTime EPGStartDate { get; set; }
    public DateTime EPGStopDate { get; set; }
    public int ProgrammeCount { get; set; }
}
