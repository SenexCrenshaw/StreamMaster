namespace StreamMaster.Domain.Extensions;
public static class MiscExtensions
{
    private static readonly Random random = new();

    public static T? GetRandomEntry<T>(this IList<T> list, int? start = 0, int? end = null)
    {
        if (list == null || list.Count == 0)
        {
            return default;
        }

        int actualStart = start ?? 0;
        int actualEnd = end ?? list.Count - 1;

        // Use Math.Clamp to ensure indices are within valid range
        actualStart = Math.Clamp(actualStart, 0, list.Count - 1);
        actualEnd = Math.Clamp(actualEnd, 0, list.Count - 1);

        // Swap actualStart and actualEnd if actualStart is greater
        if (actualStart > actualEnd)
        {
            (actualEnd, actualStart) = (actualStart, actualEnd);
        }

        int index = random.Next(actualStart, actualEnd + 1);
        return list[index];
    }
}
