using System.Text.RegularExpressions;

using StreamMaster.Domain.Services;

namespace StreamMaster.Domain.Helpers;

public partial class EPGHelper() : IEPGHelper
{
    public const int SchedulesDirectId = -1;
    public const int DummyId = -2;
    public const int CustomPlayListId = -3;
    public const int IntroPlayListId = -4;
    public const int MessageId = -5;

    public const string EPGMatch = @"(^\-?\d+)-(.*)";

    /// <summary>
    /// Attempts to extract the EPG number and station ID from the given epgId string.
    /// </summary>
    /// <param name="epgId">The EPG ID string, e.g. "12345-ABC".</param>
    /// <param name="epgNumber">When this method returns, contains the parsed EPG number, if parsing succeeded; otherwise, 0.</param>
    /// <param name="stationId">When this method returns, contains the parsed station ID, if parsing succeeded; otherwise, an empty string.</param>
    /// <returns><c>true</c> if the epgId was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryExtractEPGNumberAndStationId(string? epgId, out int epgNumber, out string stationId)
    {
        epgNumber = 0;
        stationId = string.Empty;

        if (string.IsNullOrWhiteSpace(epgId))
        {
            return false;
        }

        // Adjust this regex to match your expected EPG format.
        // For example, if epgId looks like "12345-ABC", this regex matches:
        // Group 1: digits (epgNumber)
        // Group 2: station id (non-digits)
        // Modify as needed.
        Regex pattern = MyRegex();

        Match match = pattern.Match(epgId);
        if (!match.Success || match.Groups.Count < 3)
        {
            return false;
        }

        string epgNumberStr = match.Groups[1].Value;
        string stationStr = match.Groups[2].Value;

        if (!int.TryParse(epgNumberStr, out int parsedEpgNumber))
        {
            return false;
        }

        epgNumber = parsedEpgNumber;
        stationId = stationStr;
        return true;
    }

    public (int epgNumber, string stationId) ExtractEPGNumberAndStationId(string epgId)
    {
        if (string.IsNullOrWhiteSpace(epgId))
        {
            throw new ArgumentException("Input string cannot be null or whitespace.");
        }

        MatchCollection matches = MyRegex().Matches(epgId);

        return matches.Count == 0 || !matches[0].Success || matches[0].Groups.Count != 3
            ? throw new FormatException("Input string is not in the expected format.")
            : !int.TryParse(matches[0].Groups[1].Value, out int epgNumber)
            ? throw new FormatException("Input string is not in the expected format.")
            : ((int epgNumber, string stationId))(epgNumber, matches[0].Groups[2].Value);
    }

    //public bool IsDummy(string? user_tvg_id)
    //{
    //    if (string.IsNullOrEmpty(user_tvg_id))
    //    {
    //        return true;
    //    }

    //    if (user_tvg_id.StartsWith($"{DummyId}-"))
    //    {
    //        return true;
    //    }

    //    //
    //    //bool test = IsValidEPGId(user_tvg_id);
    //    //return test || Regex.IsMatch(user_tvg_id, setting.DummyRegex, RegexOptions.IgnoreCase) || !string.IsNullOrEmpty(user_tvg_id);
    //    return false;
    //}

    //public bool IsDummy(int epgNumber)
    //{
    //    return epgNumber == DummyId;
    //}

    public bool IsSchedulesDirect(int epgNumber)
    {
        return epgNumber == SchedulesDirectId;
    }

    public static bool IsValidEPGId(string epgId)
    {
        MatchCollection matches = MyRegex().Matches(epgId);
        return matches.Count > 0;
    }

    public bool IsCustom(int epgNumber)
    {
        return epgNumber == CustomPlayListId;
    }

    [GeneratedRegex(EPGMatch)]
    private static partial Regex MyRegex();
}