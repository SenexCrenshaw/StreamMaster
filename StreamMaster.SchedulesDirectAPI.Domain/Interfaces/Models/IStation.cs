namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IStation
    {
        string Affiliate { get; set; }
        Broadcaster Broadcaster { get; set; }
        List<string> BroadcastLanguage { get; set; }
        string Callsign { get; set; }
        List<string> DescriptionLanguage { get; set; }
        bool? IsCommercialFree { get; set; }
        string Lineup { get; set; }
        Logo Logo { get; set; }
        string Id => Lineup + "|" + StationId;
        string Name { get; set; }
        string StationId { get; set; }
        List<StationLogo> StationLogo { get; set; }
    }
}