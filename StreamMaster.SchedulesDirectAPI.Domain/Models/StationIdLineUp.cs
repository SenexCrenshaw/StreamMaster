namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationIdLineup : IStationIdLineup
{
    public string Lineup { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;
    public StationIdLineup() { }
    public string Id => Lineup + "|" + StationId;

    public StationIdLineup(string lineup, string stationId)
    {
        Lineup = lineup;
        StationId = stationId;
    }
}
