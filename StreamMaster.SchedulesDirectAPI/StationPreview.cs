using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMaster.SchedulesDirectAPI;

public class StationPreview
{
    public StationPreview() { }
    public StationPreview(Station station)
    {

        Affiliate = station.Affiliate;
        Callsign = station.Callsign;
        LineUp = station.LineUp;
        Name = station.Name;
        StationId = station.StationID;
    }

    public string Affiliate { get; set; }
    public string Callsign { get; set; }
    public int Id { get; set; }
    public string LineUp { get; set; }
    public string Name { get; set; }
    public string StationId { get; set; }
}
