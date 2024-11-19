using System.Text.RegularExpressions;

using StreamMaster.Domain.Services;

namespace StreamMaster.Domain.Helpers;

public class EPGHelper() : IEPGHelper
{
    public const int SchedulesDirectId = -1;
    public const int DummyId = -2;
    public const int CustomPlayListId = -3;
    public const int IntroPlayListId = -4;
    public const int MessageId = -5;

    public const string EPGMatch = @"(^\-?\d+)-(.*)";

    public (int epgNumber, string stationId) ExtractEPGNumberAndStationId(string epgId)
    {
        if (string.IsNullOrWhiteSpace(epgId))
        {
            throw new ArgumentException("Input string cannot be null or whitespace.");
        }

        MatchCollection matches = Regex.Matches(epgId, EPGMatch);

        return matches.Count == 0 || !matches[0].Success || matches[0].Groups.Count != 3
            ? throw new FormatException("Input string is not in the expected format.")
            : !int.TryParse(matches[0].Groups[1].Value, out int epgNumber)
            ? throw new FormatException("Input string is not in the expected format.")
            : ((int epgNumber, string stationId))(epgNumber, matches[0].Groups[2].Value);
    }

    public bool IsDummy(string? user_tvg_id)
    {
        if (string.IsNullOrEmpty(user_tvg_id))
        {
            return true;
        }

        if (user_tvg_id.StartsWith($"{DummyId}-"))
        {
            return true;
        }

        //
        //bool test = IsValidEPGId(user_tvg_id);
        //return test || Regex.IsMatch(user_tvg_id, setting.DummyRegex, RegexOptions.IgnoreCase) || !string.IsNullOrEmpty(user_tvg_id);
        return false;
    }

    public bool IsDummy(int epgNumber)
    {
        return epgNumber == DummyId;
    }

    public bool IsSchedulesDirect(int epgNumber)
    {
        return epgNumber == SchedulesDirectId;
    }

    public static bool IsValidEPGId(string epgId)
    {
        MatchCollection matches = Regex.Matches(epgId, EPGMatch);
        return matches.Count > 0;
    }

    public bool IsCustom(int epgNumber)
    {
        return epgNumber == CustomPlayListId;
    }
}
