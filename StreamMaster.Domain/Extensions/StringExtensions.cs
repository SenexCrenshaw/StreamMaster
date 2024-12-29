using System.Text;

using StreamMaster.Domain.Configuration;

namespace StreamMaster.Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Validates if a given string is a valid URL with an HTTP or HTTPS scheme.
    /// </summary>
    /// <param name="url">The URL string to validate.</param>
    /// <returns>True if the URL is valid, otherwise false.</returns>
    public static bool IsValidUrl(this string url)
    {
        return !string.IsNullOrEmpty(url)
&& Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public static string ExtractPath(this string fullPath, string basePath)
    {
        if (string.IsNullOrEmpty(fullPath))
        {
            throw new ArgumentNullException(nameof(fullPath), "The full path cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(basePath))
        {
            throw new ArgumentNullException(nameof(basePath), "The base path cannot be null or empty.");
        }

        int baseIndex = fullPath.IndexOf(basePath, StringComparison.OrdinalIgnoreCase);
        return baseIndex == -1
            ? throw new ArgumentException($"Base path \"{basePath}\" not found in \"{fullPath}\".")
            : fullPath[(baseIndex + basePath.Length)..];
    }
    public static string Truncate(this string input, int maxLength)
    {
        return string.IsNullOrEmpty(input) || maxLength < 1 ? input : input.Length > maxLength ? input[..maxLength] : input;
    }
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

    public static bool IsRedirect(this string Logo)
    {
        return string.IsNullOrEmpty(Logo)
          || Logo.EqualsIgnoreCase("noimage.png")
          || Logo.EndsWithIgnoreCase("images/default.png")
          || Logo.EndsWithIgnoreCase("images/streammaster_logo.png");
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
