namespace StreamMaster.SchedulesDirect.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfLineup> _lineups = [];
    public MxfLineup FindOrCreateLineup(string lineupId, string lineupName)
    {
        if (_lineups.TryGetValue(lineupId, out MxfLineup? lineup))
        {
            return lineup;
        }
        lineup = new MxfLineup(Lineups.Count + 1, lineupId, lineupName);

        Lineups.Add(lineup);
        _lineups.Add(lineupId, lineup);
        return lineup;
    }

}
