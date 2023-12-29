namespace StreamMaster.Domain.Extensions;

public static class EPGNumberStationIdExtension
{
    public static (int epgNumber, string stationId) ExtractEPGNumberAndStationId(this string user_tvg_id)
    {
        if (string.IsNullOrWhiteSpace(user_tvg_id))
        {
            throw new ArgumentException("Input string cannot be null or whitespace.");
        }

        var parts = user_tvg_id.Split('-', 2);

        if (parts.Length != 2 || !int.TryParse(parts[0], out var number))
        {
            throw new FormatException("Input string is not in the expected format.");
        }

        return (number, parts[1]);
    }
}
