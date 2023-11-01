namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ICast
    {
        string BillingOrder { get; set; }
        string CharacterName { get; set; }
        string Name { get; set; }
        string NameId { get; set; }
        string PersonId { get; set; }
        string Role { get; set; }
    }
}