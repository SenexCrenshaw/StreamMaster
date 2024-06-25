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
        Id = Lineup + "|" + StationId;
        this.Country = station.Country;
        this.PostalCode = station.PostalCode;
    }
    public Logo Logo { get; set; }
    public string Affiliate { get; set; }
    public string Callsign { get; set; }
    public string Id { get; set; }
    public string Lineup { get; set; }
    public string Name { get; set; }
    public string StationId { get; set; }
    public string Country { get; set; }
    public string PostalCode { get; set; }
}
