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
        string LineUp { get; set; }
        Logo Logo { get; set; }
        string Name { get; set; }
        string StationID { get; set; }
        List<StationLogo> StationLogo { get; set; }
    }
}