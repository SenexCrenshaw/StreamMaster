namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IStationPreview
    {
        string Affiliate { get; set; }
        string Callsign { get; set; }
        string Id => Lineup + "|" + StationId;
        string Lineup { get; set; }
        Logo Logo { get; set; }
        string Name { get; set; }
        string StationId { get; set; }
    }
}