namespace StreamMaster.Domain.Exceptions;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        return size <= 0 ? throw new ArgumentOutOfRangeException(nameof(size), "Batch size must be greater than 0.") : BatchInternal();
        IEnumerable<IEnumerable<T>> BatchInternal()
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return YieldBatchElements(enumerator, size - 1);
            }
        }
    }

    private static IEnumerable<T> YieldBatchElements<T>(IEnumerator<T> enumerator, int size)
    {
        yield return enumerator.Current;
        for (int i = 0; i < size && enumerator.MoveNext(); i++)
        {
            yield return enumerator.Current;
        }
    }


}
