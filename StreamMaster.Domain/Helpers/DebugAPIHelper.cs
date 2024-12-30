using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace StreamMaster.Domain.Helpers
{
    public static class DebugAPIHelper
    {
        public static async Task<T> DebugAPI<T>(
        Task<T> task,
        ILogger logger,
        bool isDebugEnabled,
        [CallerMemberName] string callerName = "")
        {
            if (isDebugEnabled)
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                logger.LogInformation("{callerName} Started", callerName);

                T? result = await task.ConfigureAwait(false);

                stopwatch.Stop();
                try
                {
                    int byteSize = JsonSerializer.SerializeToUtf8Bytes(result).Length;

                    string readableSize = FormatBytes(byteSize);
                    string elapsedTime = FormatElapsedTime(stopwatch.ElapsedMilliseconds);

                    logger.LogInformation("{callerName} retrieved size: {readableSize}, in {elapsedTime}", callerName, readableSize, elapsedTime);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to serialize debug data in {callerName}", callerName);
                }

                return result;
            }

            return await task.ConfigureAwait(false);
        }

        private static string FormatBytes(long byteCount)
        {
            if (byteCount < 1024)
            {
                return $"{byteCount} bytes";
            }

            return byteCount < 1048576
                ? $"{byteCount / 1024.0:F2} KB"
                : byteCount < 1073741824 ? $"{byteCount / 1048576.0:F2} MB" : $"{byteCount / 1073741824.0:F2} GB";
        }

        private static string FormatElapsedTime(long milliseconds)
        {
            return milliseconds < 1000
                ? $"{milliseconds} ms"
                : milliseconds < 60000 ? $"{milliseconds / 1000.0:F2} seconds" : $"{milliseconds / 60000.0:F2} minutes";
        }
    }
}
