namespace StreamMaster.SchedulesDirectAPI.Data;

public partial class SchedulesDirectData
{
    private Dictionary<string, MxfLineup> _lineups = [];
    public MxfLineup FindOrCreateLineup(string lineupId, string lineupName, int? epgId = null)
    {
        if (_lineups.TryGetValue(lineupId, out MxfLineup? lineup))
        {
            return lineup;
        }
        lineup = new MxfLineup(Lineups.Count + 1, lineupId, lineupName);
        if (epgId != null)
        {
            lineup.extras.Add("epgid", epgId);
        }

        Lineups.Add(lineup);
        _lineups.Add(lineupId, lineup);
        return lineup;
    }

}
