using StreamMaster.Domain.Mappings;

namespace StreamMaster.SchedulesDirect.Domain.Dto;
[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class HeadendDto : IMapFrom<Headend>
{
    public string Id { get; set; } = string.Empty;
    public string HeadendId { get; set; } = string.Empty;
    public string Lineup { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Transport { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;

    //public bool SubScribed { get; set; } = false;

}
