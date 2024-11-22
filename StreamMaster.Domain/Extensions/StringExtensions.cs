using System.Text;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Extensions;

public static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string text, string Compare)
    {
        return text.Equals(Compare, BuildInfo.StringComparison);
    }

    public static string ToBase64String(this string text)
    {
        string ret = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        return ret;
    }

    public static bool IsBase64String(this string base64)
    {
        if (string.IsNullOrEmpty(base64) || base64.Length % 4 != 0)
        {
            return false;
        }

        try
        {
            _ = Convert.FromBase64String(base64);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool ContainsIgnoreCase(this string text, string Contains)
    {
        return text.Contains(Contains, BuildInfo.StringComparison);
    }

    public static bool StartsWithIgnoreCase(this string text, string StartsWith)
    {
        return text.StartsWith(StartsWith, BuildInfo.StringComparison);
    }

    public static bool EndsWithIgnoreCase(this string text, string EndsWith)
    {
        return text.EndsWith(EndsWith, BuildInfo.StringComparison);
    }
}
