using System.Globalization;
using System.Text.RegularExpressions;

namespace DateTester
{
    internal class Program
    {
        private static DateTime ParseFlexibleDate(string dateStr)
        {
            // Regex to capture parts of the date string
            Regex regex = new(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{1,2})(?<minute>\d{2})(?<second>\d{2})?\s*(?<offset>[+-]\d{4})");
            Match match = regex.Match(dateStr);

            if (!match.Success)
            {
                throw new ArgumentException($"Invalid date format: {dateStr}");
            }

            // Extract parts or default values if missing
            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            int day = int.Parse(match.Groups["day"].Value);
            int hour = int.Parse(match.Groups["hour"].Value);
            int minute = int.Parse(match.Groups["minute"].Value);
            int second = match.Groups["second"].Success ? int.Parse(match.Groups["second"].Value) : 0; // Default to 0 if missing

            // Adjust for hours >= 24 by moving to the next day
            if (hour >= 24)
            {
                hour -= 24;
                day += 1;
            }

            // Normalize the date string to "yyyyMMddHHmmss zzz" format
            string normalizedDateStr = $"{year:D4}{month:D2}{day:D2}{hour:D2}{minute:D2}{second:D2}";

            // Adjust offset format from "+0100" to "+01:00" if necessary
            string offsetStr = match.Groups["offset"].Value;
            if (offsetStr.Length == 5)
            {
                offsetStr = offsetStr.Insert(3, ":"); // Transform "+0100" to "+01:00"
            }

            // Parse with DateTimeOffset using normalized string
            DateTimeOffset parsedDate = DateTimeOffset.ParseExact($"{normalizedDateStr} {offsetStr}", "yyyyMMddHHmmss zzz", CultureInfo.InvariantCulture);
            return parsedDate.UtcDateTime;
        }




        private static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            DateTime test = ParseFlexibleDate("2024102900000 +0100");
        }
    }
}
