using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    [XmlArrayItem("Lineup")]
    public ConcurrentDictionary<string, MxfLineup> Lineups { get; set; } = new();

    public MxfLineup FindOrCreateLineup(string lineupId, string lineupName)
    {
        return Lineups.FindOrCreate(lineupId, key => new MxfLineup(Lineups.Count + 1, lineupId, lineupName));
    }

    public void RemoveLineup(string lineupId)
    {
        if (!Lineups.ContainsKey(lineupId))
        {
            Console.WriteLine($"Lineup not found {lineupId}");
            return;
        }
        MxfLineup lineup = Lineups[lineupId];
        if (lineup == null)
        {
            Console.WriteLine($"Lineup not found {lineupId}");
            return;
        }

        Lineups.TryRemove(lineupId, out _);


    }
}
