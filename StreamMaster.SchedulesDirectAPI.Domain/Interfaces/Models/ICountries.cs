using StreamMaster.SchedulesDirectAPI.Domain.Models;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface ICountries
    {
        List<Caribbean> Caribbean { get; set; }
        List<Europe> Europe { get; set; }
        List<LatinAmerica> LatinAmerica { get; set; }
        List<NorthAmerica> NorthAmerica { get; set; }
        List<Oceanium> Oceania { get; set; }
    }
}