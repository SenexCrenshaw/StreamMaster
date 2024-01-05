using StreamMaster.SchedulesDirect.Domain.Enums;

using System.Collections.Concurrent;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISchedulesDirectAPIService
    {
        Task DownloadImageResponsesAsync(List<string> imageQueue, ConcurrentBag<ProgramMetadata> programMetadata, int start = 0);
        bool CheckToken(bool forceReset = false);
        Task<HttpResponseMessage?> GetSdImage(string uri);
        Task<T?> GetApiResponse<T>(APIMethod method, string uri, object? classObject = null, CancellationToken cancellationToken = default);
    }
}