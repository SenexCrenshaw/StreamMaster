using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Lineup")]
    public ConcurrentDictionary<string, MxfLineup> Lineups { get; set; } = new();

    public MxfLineup FindOrCreateLineup(string lineupId, string lineupName)
    {
        return Lineups.FindOrCreate(lineupId, _ => new MxfLineup(Lineups.Count + 1, lineupId, lineupName));
    }

    public void RemoveLineup(string lineupId)
    {
        if (!Lineups.TryRemove(lineupId, out MxfLineup? lineup) || lineup == null)
        {
            Console.WriteLine($"Lineup not found: {lineupId}");
        }
    }
}
