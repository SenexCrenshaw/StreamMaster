namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface IEurope
    {
        string FullName { get; set; }
        bool? OnePostalCode { get; set; }
        string PostalCode { get; set; }
        string PostalCodeExample { get; set; }
        string ShortName { get; set; }
    }
}