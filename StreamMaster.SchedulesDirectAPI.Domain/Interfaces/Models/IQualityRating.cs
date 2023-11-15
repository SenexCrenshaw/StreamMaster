namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IQualityRating
    {
        string Increment { get; set; }
        string MaxRating { get; set; }
        string MinRating { get; set; }
        string Rating { get; set; }
        string RatingsBody { get; set; }
    }
}