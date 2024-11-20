namespace StreamMaster.Domain.Color;
public static class ColorHelper
{
    public static string GetColorHex(int index)
    {
        string hsl = GetColorByNumber(index);
        int[] hslValues = hsl[4..^1].Split(',').Select(int.Parse).ToArray();
        if (hslValues.Length == 3)
        {
            (int r, int g, int b) = HslToRgb(hslValues[0], hslValues[1], hslValues[2]);
            return RgbToHex(r, g, b);
        }
        return "#000000"; // Fallback color
    }

    public static string GetColor(string text)
    {
        int hash = StringToHash(text);
        int hue = hash % 360;
        int saturation = (hash % 30) + 70; // Values between 70% and 100%
        int lightness = (hash % 20) + 65; // Values between 65% and 85%
        return $"hsl({hue}, {saturation}%, {lightness}%)";
    }

    private static string GetColorByNumber(int index)
    {
        const int Step = 50; // This will determine the number of segments in the hue spectrum
        const int segments = 360 / Step;

        // Calculate a hue value that jumps around the spectrum to ensure variation
        int hue = ((index * Step) + (index / segments * Step)) % 360;

        return $"hsl({hue}, 100%, 70%)";
    }

    private static int StringToHash(string str)
    {
        int hash = 0;
        foreach (char c in str)
        {
            hash = (hash << 5) - hash + c;
            hash &= hash; // Convert to 32bit integer
        }

        return Math.Abs(hash);
    }

    private static (int, int, int) HslToRgb(int h, int s, int l)
    {
        double r = 0, g = 0, b = 0;
        double sd = s / 100.0;
        double ld = l / 100.0;

        double c = (1 - Math.Abs((2 * ld) - 1)) * sd;
        double x = c * (1 - Math.Abs((h / 60.0 % 2) - 1));
        double m = ld - (c / 2);

        if (h is >= 0 and < 60)
        {
            r = c;
            g = x;
            b = 0;
        }
        else if (h is >= 60 and < 120)
        {
            r = x;
            g = c;
            b = 0;
        }
        else if (h is >= 120 and < 180)
        {
            r = 0;
            g = c;
            b = x;
        }
        else if (h is >= 180 and < 240)
        {
            r = 0;
            g = x;
            b = c;
        }
        else if (h is >= 240 and < 300)
        {
            r = x;
            g = 0;
            b = c;
        }
        else if (h is >= 300 and < 360)
        {
            r = c;
            g = 0;
            b = x;
        }

        return ((int)Math.Round((r + m) * 255), (int)Math.Round((g + m) * 255), (int)Math.Round((b + m) * 255));
    }

    private static string RgbToHex(int r, int g, int b)
    {
        return "#" + ((r << 16) + (g << 8) + b).ToString("X6").PadLeft(6, '0');
    }
}