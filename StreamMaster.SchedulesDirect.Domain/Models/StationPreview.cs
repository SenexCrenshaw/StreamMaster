namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationPreview
{
    public StationPreview() { }
    public StationPreview(Station station)
    {

        Affiliate = station.Affiliate;
        Callsign = station.Callsign;
        Lineup = station.Lineup;
        Name = station.Name;
        StationId = station.StationId;
        Logo = station.Logo;
    }
    public Logo Logo { get; set; }
    public string Affiliate { get; set; }
    public string Callsign { get; set; }
    public string Id => Lineup + "|" + StationId;
    public string Lineup { get; set; }
    public string Name { get; set; }
    public string StationId { get; set; }
}
