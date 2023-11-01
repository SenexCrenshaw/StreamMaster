namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationPreview : IStationPreview
{
    public StationPreview() { }
    public StationPreview(IStation station)
    {

        Affiliate = station.Affiliate;
        Callsign = station.Callsign;
        LineUp = station.LineUp;
        Name = station.Name;
        StationId = station.StationID;
        Logo = station.Logo;
    }
    public Logo Logo { get; set; }
    public string Affiliate { get; set; }
    public string Callsign { get; set; }
    public int Id { get; set; }
    public string LineUp { get; set; }
    public string Name { get; set; }
    public string StationId { get; set; }
}
