namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IMovie
    {
        int Duration { get; set; }
        List<QualityRating> QualityRating { get; set; }
        string Year { get; set; }
    }
}