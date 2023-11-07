namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ILineupsResult
    {
        int Code { get; set; }
        DateTime Datetime { get; set; }
        List<Lineup> Lineups { get; set; }
        string ServerID { get; set; }
    }
}