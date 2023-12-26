namespace StreamMaster.SchedulesDirect.Helpers;

public static class SDExtensions
{
    public static List<string>? CheckStatus(this UserStatus status)
    {
        List<string> ret = [];

        foreach (StatusLineup lineup in status.Lineups)
        {
            if (lineup.IsDeleted)
            {
                ret.Add($"Lineup {lineup.Lineup} is deleted");
            }
        }

        return ret.Any() ? ret : null;
    }

}