namespace StreamMaster.SchedulesDirect.Domain.Models;

public class CountryData
{
    public string Id { get => Key; }
    public string Key { get; set; }
    public List<Country> Countries { get; set; }
}


