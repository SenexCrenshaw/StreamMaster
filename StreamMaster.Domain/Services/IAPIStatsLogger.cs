using System.Runtime.CompilerServices;

namespace StreamMaster.Domain.Services
{
    public interface IAPIStatsLogger
    {
        Task<T> DebugAPI<T>(Task<T> task, [CallerMemberName] string callerName = "");
        Task WriteToLogFileAsync(string functionName, string size, string elapsedTime, string message);
    }
}