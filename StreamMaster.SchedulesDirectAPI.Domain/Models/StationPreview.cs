namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationPreview : IStationPreview
{
    public StationPreview() { }
    public StationPreview(IStation station)
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
