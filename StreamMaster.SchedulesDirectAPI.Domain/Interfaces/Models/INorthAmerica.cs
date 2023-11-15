namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces.Models
{
    public interface INorthAmerica
    {
        string FullName { get; set; }
        string PostalCode { get; set; }
        string PostalCodeExample { get; set; }
        string ShortName { get; set; }
    }
}