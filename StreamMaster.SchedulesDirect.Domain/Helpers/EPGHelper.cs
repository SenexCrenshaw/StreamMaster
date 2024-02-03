using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirect.Domain.Helpers;

public static class EPGChecks
{
    public const string EPGMatch = @"(^\-?\d+)-(.*)";

    public static bool IsValidEPGId(string epgId)
    {
        MatchCollection matches = Regex.Matches(epgId, EPGMatch);
        return matches.Count > 0;
    }

}
