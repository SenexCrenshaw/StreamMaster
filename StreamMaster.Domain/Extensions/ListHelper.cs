using System.Reflection;
using System.Text.RegularExpressions;

namespace StreamMaster.Domain.Extensions;

public static class ListHelper
{
    public static async Task ForEachAsync<T>(
    this IAsyncEnumerable<T> source,
    int degreeOfParallelism,
    Func<T, Task> body,
    CancellationToken cancellationToken = default)
    {
        SemaphoreSlim semaphore = new(degreeOfParallelism);

        List<Task> tasks = [];

        await foreach (T? item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            Task task = Task.Run(async () =>
            {
                try
                {
                    await body(item).ConfigureAwait(false);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public static List<T> GetMatchingProperty<T>(List<T> list, string propertyName, string regex)
    {
        List<T> matchedObjects = [];
        Regex rgx = new(regex, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);

        PropertyInfo? property = typeof(T).GetProperty(propertyName) ?? throw new ArgumentException("No such property found", nameof(propertyName));

        foreach (T? obj in list)
        {
            object? value = property.GetValue(obj, null);
            if (value != null)
            {
                string? stringValue = value.ToString();
                if (rgx.IsMatch(stringValue!))
                {
                    matchedObjects.Add(obj);
                }
            }
        }

        return matchedObjects;
    }
}
