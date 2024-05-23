using Reinforced.Typings.Attributes;

namespace StreamMaster.Domain.Models;

[TsInterface(AutoI = false, IncludeNamespace = false, FlattenHierarchy = true, AutoExportMethods = false)]
public class StationIdLineup
{
    public string Lineup { get; set; } = string.Empty;
    public string StationId { get; set; } = string.Empty;
    public StationIdLineup() { }
    public string Id => Lineup + "|" + StationId;

    public StationIdLineup(string stationId, string lineup)
    {
        Lineup = lineup;
        StationId = stationId;
    }

}
