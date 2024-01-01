using Microsoft.Extensions.Caching.Memory;

using StreamMaster.Domain.Services;

using System.Text.RegularExpressions;

namespace StreamMaster.SchedulesDirect.Helpers;

public class EPGHelper(IMemoryCache memoryCache) : IEPGHelper
{
    public const int SchedulesDirectId = -1;
    public const int DummyId = -2;
    public const string EPGMatch = @"(^\-?\d+)-(.*)";

    public (int epgNumber, string stationId) ExtractEPGNumberAndStationId(string epgId)
    {
        if (string.IsNullOrWhiteSpace(epgId))
        {
            throw new ArgumentException("Input string cannot be null or whitespace.");
        }

        var matches = Regex.Matches(epgId, EPGMatch);

        if (matches.Count == 0 || !matches[0].Success || matches[0].Groups.Count != 3)
        {
            throw new FormatException("Input string is not in the expected format.");
        }

        if (!int.TryParse(matches[0].Groups[1].Value, out var epgNumber))
        {
            throw new FormatException("Input string is not in the expected format.");
        }

        return (epgNumber, matches[0].Groups[2].Value);
    }

    public bool IsDummy(string? user_tvg_id)
    {
        if (string.IsNullOrEmpty(user_tvg_id))
        {
            return true;
        }

        if (user_tvg_id.StartsWith("DUMMY", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var setting = memoryCache.GetSetting();

        if (Regex.IsMatch(user_tvg_id, setting.DummyRegex, RegexOptions.IgnoreCase))
        {
            return true;
        }

        return !IsValidEPGId(user_tvg_id);

    }

    public bool IsDummy(int epgNumber)
    {
        return epgNumber == DummyId;
    }

    public bool IsSchedulesDirect(int epgNumber)
    {
        return epgNumber == SchedulesDirectId;
    }

    public bool IsValidEPGId(string epgId)
    {
        var matches = Regex.Matches(epgId, EPGMatch);
        return matches.Count > 0;
    }

}
