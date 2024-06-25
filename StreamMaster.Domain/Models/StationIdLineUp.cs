namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationIdLineup
{
    public StationIdLineup() { }
    public string Lineup { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;

    //public StationIdLineup(string stationId, string lineup, string Country, string PostalCode)
    public StationIdLineup(string StationId, string Lineup)
    {
        this.Lineup = Lineup;
        this.StationId = StationId;
        //this.Country = Country;
        //this.PostalCode = PostalCode;
        this.Id = Lineup + "|" + StationId;
    }

}
