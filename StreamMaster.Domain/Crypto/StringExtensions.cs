using System.Web;

namespace StreamMaster.Domain.Crypto;
public static class StringExtensions
{
    public static string ToCleanFileString(this string input)
    {
        return string.IsNullOrWhiteSpace(input)
            ? throw new ArgumentException("Input cannot be null or whitespace.", nameof(input))
            : HttpUtility.HtmlEncode(input).Trim().Replace('+', '-').Replace(' ', '_').Replace('/', '_');
    }

}
