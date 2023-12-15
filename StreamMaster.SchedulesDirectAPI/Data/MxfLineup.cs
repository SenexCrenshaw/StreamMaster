namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfLineup> _lineups = [];
    public MxfLineup FindOrCreateLineup(string lineupId, string lineupName)
    {
        if (_lineups.TryGetValue(lineupId, out MxfLineup? lineup))
        {
            return lineup;
        }

        Lineups.Add(lineup = new MxfLineup(Lineups.Count + 1, lineupId, lineupName));
        _lineups.Add(lineupId, lineup);
        return lineup;
    }

}
