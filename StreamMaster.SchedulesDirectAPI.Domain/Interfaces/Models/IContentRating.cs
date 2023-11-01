namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IContentRating
    {
        string Body { get; set; }
        string Code { get; set; }
        List<string> ContentAdvisory { get; set; }
        string Country { get; set; }
    }
}