namespace StreamMaster.SchedulesDirectAPI.Domain.Models;

public class StationIdLineUp : IStationIdLineUp
{
    public string LineUp { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;
    public StationIdLineUp() { }
    public string Id => LineUp + "|" + StationId;

    public StationIdLineUp(string lineUp, string stationId)
    {
        LineUp = lineUp;
        StationId = stationId;
    }
}
