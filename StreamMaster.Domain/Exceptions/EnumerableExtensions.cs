namespace StreamMaster.Domain.Exceptions;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);

        return size <= 0 ? throw new ArgumentOutOfRangeException(nameof(size), "Batch size must be greater than 0.") : Batch2();
        IEnumerable<IEnumerable<T>> Batch2()
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return YieldBatchElements(enumerator, size - 1);
            }
        }
    }

    private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> source, int size)
    {
        yield return source.Current;
        for (int i = 0; i < size && source.MoveNext(); i++)
        {
            yield return source.Current;
        }
    }
}
