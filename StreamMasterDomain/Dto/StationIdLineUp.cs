namespace StreamMasterDomain.Dto;

public class StationIdLineUp
{
    public string LineUp { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;
    public StationIdLineUp() { }

    public StationIdLineUp(string lineUp, string stationId)
    {
        LineUp = lineUp;
        StationId = stationId;
    }
}
