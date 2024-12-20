namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class CountryData
{
    public string? Id => Key;
    public string? Key { get; set; }
    public List<Country>? Countries { get; set; }
}