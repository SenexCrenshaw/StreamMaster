using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Helpers;

public static class EPGNumberStationIdExtension
{
    public static (int epgNumber, string stationId) ExtractEPGNumberAndStationId(this string user_tvg_id)
    {
        if (string.IsNullOrWhiteSpace(user_tvg_id))
        {
            throw new ArgumentException("Input string cannot be null or whitespace.");
        }


        MatchCollection matches = Regex.Matches(user_tvg_id, EPGHelper.EPGMatch);

        return matches.Count == 0 || !matches[0].Success || matches[0].Groups.Count != 3
            ? throw new FormatException("Input string is not in the expected format.")
            : !int.TryParse(matches[0].Groups[1].Value, out int epgNumber)
            ? throw new FormatException("Input string is not in the expected format.")
            : ((int epgNumber, string stationId))(epgNumber, matches[0].Groups[2].Value);
    }
}
