namespace StreamMaster.Domain.Extensions;

public static class TaskCompletionSourceExtensions
{
    public static async Task<bool> WaitWithTimeoutAsync(this TaskCompletionSource<bool> source, string trackingName, int timeoutMilliseconds, CancellationToken cancellationToken)
    {
        if (source.Task.IsCompleted)
        {
            // If the signal is already set, return immediately.
            return await source.Task.ConfigureAwait(false);
        }

        // Create a delay task for the timeout.
        Task delayTask = Task.Delay(timeoutMilliseconds, cancellationToken);

        // Wait for either the signal to be set or the delay to complete.
        Task completedTask = await Task.WhenAny(source.Task, delayTask).ConfigureAwait(false);

        // If the signal task is completed, return its result.
        if (completedTask == source.Task)
        {
            return await source.Task.ConfigureAwait(false);
        }

        // If the delay task is completed, it means we timed out.
        throw new TimeoutException($"{trackingName} timed out in {timeoutMilliseconds} ms");
    }
}
