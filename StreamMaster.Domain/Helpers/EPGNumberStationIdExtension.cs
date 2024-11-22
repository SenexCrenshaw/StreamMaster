using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Helpers;

public static partial class EPGNumberStationIdExtension
{
    /// <summary>
    /// Extracts the EPG number and station ID from a given string.
    /// </summary>
    /// <param name="userTvgId">The input string to parse.</param>
    /// <returns>
    /// A tuple containing the EPG number and station ID, or null if parsing fails.
    /// </returns>
    public static (int epgNumber, string stationId) ExtractEPGNumberAndStationId(this string userTvgId)
    {
        if (string.IsNullOrWhiteSpace(userTvgId))
        {
            return (EPGHelper.DummyId, "Dummy");
        }

        // Define or retrieve your regex here
        Regex regex = MyRegex();

        // Match the input string with the regex
        Match match = regex.Match(userTvgId);

        // Validate the match and parse the values
        if (!match.Success || match.Groups.Count != 3)
        {
            return (EPGHelper.DummyId, userTvgId);
        }

        if (int.TryParse(match.Groups[1].Value, out int epgNumber))
        {
            string stationId = match.Groups[2].Value;
            return (epgNumber, stationId);
        }

        return (EPGHelper.DummyId, "Dummy");
    }

    [GeneratedRegex(EPGHelper.EPGMatch)]
    private static partial Regex MyRegex();
}
