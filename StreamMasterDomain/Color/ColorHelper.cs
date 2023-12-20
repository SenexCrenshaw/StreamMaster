namespace StreamMasterDomain.Color;
public static class ColorHelper
{
    public static string GetColor(string text)
    {
        int hash = StringToHash(text);
        int hue = hash % 360;
        int saturation = (hash % 30) + 70; // Values between 70% and 100%
        int lightness = (hash % 20) + 65; // Values between 65% and 85%

        return HslToHex(hue, saturation, lightness);
    }

    private static int StringToHash(string str)
    {
        unchecked
        {
            int hash = 23;
            foreach (char c in str)
            {
                hash = (hash * 31) + c;
            }
            return Math.Abs(hash);
        }
    }

    private static string HslToHex(int h, int s, int l)
    {
        (float r, float g, float b) = HslToRgb(h, s, l);
        return $"#{(int)(r * 255):X2}{(int)(g * 255):X2}{(int)(b * 255):X2}";
    }

    private static (float, float, float) HslToRgb(int h, int s, int l)
    {
        float r, g, b;
        float saturation = s / 100f;
        float lightness = l / 100f;

        if (saturation == 0)
        {
            r = g = b = lightness; // Achromatic color (gray).
        }
        else
        {
            float q = lightness < 0.5 ? lightness * (1 + saturation) : lightness + saturation - (lightness * saturation);
            float p = (2 * lightness) - q;
            r = HueToRgb(p, q, (h / 360f) + (1f / 3f));
            g = HueToRgb(p, q, h / 360f);
            b = HueToRgb(p, q, (h / 360f) - (1f / 3f));
        }

        return (r, g, b);
    }

    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1f / 6f) return p + ((q - p) * 6 * t);
        return t < 1f / 2f ? q : t < 2f / 3f ? p + ((q - p) * ((2f / 3f) - t) * 6) : p;
    }
}
