using System.Globalization;
using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Helpers
{
    public static class ParseFlexibleDateHelper
    {
        public static DateTime ParseFlexibleDate(string dateStr)
        {
            // Regex to capture parts of the date string with optional components
            Regex regex = new(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})?(?<minute>\d{2})?(?<second>\d{2})?\s*(?<offset>[+-]\d{4})?");
            Match match = regex.Match(dateStr);

            if (!match.Success)
            {
                throw new ArgumentException($"Invalid date format: {dateStr}");
            }

            // Extract parts or default values if missing
            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            int day = int.Parse(match.Groups["day"].Value);
            int hour = match.Groups["hour"].Success ? int.Parse(match.Groups["hour"].Value) : 0; // Default to 0 if missing
            int minute = match.Groups["minute"].Success ? int.Parse(match.Groups["minute"].Value) : 0; // Default to 0 if missing
            int second = match.Groups["second"].Success ? int.Parse(match.Groups["second"].Value) : 0; // Default to 0 if missing

            // Handle hour overflow (e.g., hour >= 24 means add to the next day)
            while (hour >= 24)
            {
                hour -= 24;
                day += 1;
            }

            // Handle minute overflow (e.g., minute >= 60)
            while (minute >= 60)
            {
                minute -= 60;
                hour += 1;
            }

            // Handle second overflow (e.g., second >= 60)
            while (second >= 60)
            {
                second -= 60;
                minute += 1;
            }

            // Adjust day if it exceeds the maximum for the month
            while (!IsValidDay(year, month, day))
            {
                int maxDay = DateTime.DaysInMonth(year, month);
                if (day > maxDay)
                {
                    day -= maxDay;
                    month += 1;

                    // If the month exceeds December, roll over to January of the next year
                    if (month > 12)
                    {
                        month = 1;
                        year += 1;
                    }
                }
            }

            // Create a DateTime object using the adjusted values
            DateTime dateTime = new(year, month, day, hour, minute, second);

            // Handle offset if available
            if (match.Groups["offset"].Success)
            {
                string offsetStr = match.Groups["offset"].Value;

                // Adjust offset format from "+0200" to "+02:00" if necessary
                if (offsetStr.Length == 5)
                {
                    offsetStr = offsetStr.Insert(3, ":"); // Transform "+0100" to "+01:00"
                }

                // Parse the offset, handling both positive and negative offsets
                bool isNegative = offsetStr.StartsWith("-");
                offsetStr = offsetStr.TrimStart('+', '-'); // Remove leading '+' or '-'
                TimeSpan offset = TimeSpan.ParseExact(offsetStr, "hh\\:mm", CultureInfo.InvariantCulture);

                // Apply the negative sign if required
                if (isNegative)
                {
                    offset = offset.Negate();
                }

                // Create a DateTimeOffset and convert it to UTC
                DateTimeOffset dateTimeOffset = new(dateTime, offset);
                return dateTimeOffset.UtcDateTime;
            }

            // If no offset is provided, assume local time or treat it as UTC as per requirement
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }

        // Helper function to validate day value for a given month and year
        private static bool IsValidDay(int year, int month, int day)
        {
            int maxDay = DateTime.DaysInMonth(year, month);
            return day >= 1 && day <= maxDay;
        }
    }
}
