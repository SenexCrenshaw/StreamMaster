namespace StreamMaster.Domain.Color;

/// <summary>
/// Provides utility methods for working with colors.
/// </summary>
public static class ColorHelper
{
    private const int Step = 50; // Determines hue step size for color segmentation.

    /// <summary>
    /// Gets a hex color code based on an index.
    /// </summary>
    /// <param name="index">The index for color selection.</param>
    /// <returns>Hexadecimal color code.</returns>
    public static string GetColorHex(int index)
    {
        string hsl = GetColorByNumber(index);
        (int h, int s, int l)? hslValues = ParseHslValues(hsl);

        if (hslValues != null)
        {
            (int r, int g, int b) = HslToRgb(hslValues.Value.h, hslValues.Value.s, hslValues.Value.l);
            return RgbToHex(r, g, b);
        }

        return "#000000"; // Fallback color.
    }

    /// <summary>
    /// Generates an HSL color string based on input text.
    /// </summary>
    /// <param name="text">Input text.</param>
    /// <returns>HSL color string.</returns>
    public static string GetColor(string text)
    {
        int hash = StringToHash(text);
        int hue = hash % 360;
        int saturation = (hash % 30) + 70; // Values between 70% and 100%.
        int lightness = (hash % 20) + 65; // Values between 65% and 85%.

        return $"hsl({hue}, {saturation}%, {lightness}%)";
    }

    /// <summary>
    /// Generates an HSL color string based on an index.
    /// </summary>
    /// <param name="index">The index for color selection.</param>
    /// <returns>HSL color string.</returns>
    private static string GetColorByNumber(int index)
    {
        const int segments = 360 / Step;
        int hue = ((index * Step) + (index / segments * Step)) % 360;

        return $"hsl({hue}, 100%, 70%)";
    }

    /// <summary>
    /// Converts an HSL color to RGB.
    /// </summary>
    /// <param name="h">Hue (0-360).</param>
    /// <param name="s">Saturation (0-100).</param>
    /// <param name="l">Lightness (0-100).</param>
    /// <returns>RGB color values.</returns>
    private static (int r, int g, int b) HslToRgb(int h, int s, int l)
    {
        double sd = s / 100.0;
        double ld = l / 100.0;

        double c = (1 - Math.Abs((2 * ld) - 1)) * sd;
        double x = c * (1 - Math.Abs((h / 60.0 % 2) - 1));
        double m = ld - (c / 2);

        double r = 0, g = 0, b = 0;

        if (h < 60) { r = c; g = x; }
        else if (h < 120) { r = x; g = c; }
        else if (h < 180) { g = c; b = x; }
        else if (h < 240) { g = x; b = c; }
        else if (h < 300) { r = x; b = c; }
        else { r = c; b = x; }

        return (
            (int)Math.Round((r + m) * 255),
            (int)Math.Round((g + m) * 255),
            (int)Math.Round((b + m) * 255)
        );
    }

    /// <summary>
    /// Converts RGB color values to a hexadecimal color code.
    /// </summary>
    /// <param name="r">Red (0-255).</param>
    /// <param name="g">Green (0-255).</param>
    /// <param name="b">Blue (0-255).</param>
    /// <returns>Hexadecimal color code.</returns>
    private static string RgbToHex(int r, int g, int b)
    {
        return $"#{r:X2}{g:X2}{b:X2}";
    }

    /// <summary>
    /// Generates a hash from a string.
    /// </summary>
    /// <param name="str">Input string.</param>
    /// <returns>Non-negative hash value.</returns>
    private static int StringToHash(string str)
    {
        int hash = 0;
        foreach (char c in str)
        {
            hash = (hash << 5) - hash + c;
        }
        return Math.Abs(hash);
    }

    /// <summary>
    /// Parses HSL values from a string.
    /// </summary>
    /// <param name="hsl">HSL string in the format "hsl(h, s%, l%)".</param>
    /// <returns>Parsed HSL values or null if invalid.</returns>
    private static (int h, int s, int l)? ParseHslValues(string hsl)
    {
        if (hsl.StartsWith("hsl(", StringComparison.OrdinalIgnoreCase) && hsl.EndsWith(')'))
        {
            ReadOnlySpan<char> hslContent = hsl.AsSpan(4, hsl.Length - 5);
            List<string> parts = [];
            foreach (Range part in hslContent.Split(','))
            {
                parts.Add(part.ToString().Trim());
            }

            if (parts.Count == 3 &&
                int.TryParse(parts[0].Trim(), out int h) &&
                int.TryParse(parts[1].TrimEnd('%'), out int s) &&
                int.TryParse(parts[2].TrimEnd('%'), out int l))
            {
                return (h, s, l);
            }
        }

        return null;
    }
}