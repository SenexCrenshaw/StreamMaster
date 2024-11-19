namespace StreamMaster.SchedulesDirect.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationPreview
{
    public StationPreview() { }
    public StationPreview(Station station)
    {
        if (!string.IsNullOrEmpty(station.Affiliate))
        {
            Affiliate = station.Affiliate;
        }

        if (!string.IsNullOrEmpty(station.Callsign))
        {
            Callsign = station.Callsign;
        }

        if (!string.IsNullOrEmpty(station.Lineup))
        {
            Lineup = station.Lineup;
        }

        if (!string.IsNullOrEmpty(station.Name))
        {
            Name = station.Name;
        }

        if (!string.IsNullOrEmpty(station.StationId))
        {
            StationId = station.StationId;
        }

        if (station.Logo is not null)
        {
            Logo = station.Logo;
        }

        // Create a composite Id using Lineup and StationId
        Id = $"{Lineup}|{StationId}";

        // Assign Country if it’s not null or empty
        if (!string.IsNullOrEmpty(station.Country))
        {
            Country = station.Country;
        }

        // Assign PostalCode if it’s not null or empty
        if (!string.IsNullOrEmpty(station.PostalCode))
        {
            PostalCode = station.PostalCode;
        }

    }

    public Logo? Logo { get; set; }
    public string? Affiliate { get; set; }
    public string? Callsign { get; set; }
    public string? Id { get; set; }
    public string? Lineup { get; set; }
    public string? Name { get; set; }
    public string? StationId { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
}
