namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IAward
    {
        string AwardName { get; set; }
        string Category { get; set; }
        string Name { get; set; }
        string PersonId { get; set; }
        string Recipient { get; set; }
        bool? Won { get; set; }
        string Year { get; set; }
    }
}