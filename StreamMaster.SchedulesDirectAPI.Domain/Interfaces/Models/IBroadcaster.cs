namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IBroadcaster
    {
        string City { get; set; }
        string Country { get; set; }
        string Postalcode { get; set; }
        string State { get; set; }
    }
}