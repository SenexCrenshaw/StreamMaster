using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface ISchedulesDirectAPIService
    {
        bool CheckToken(bool forceReset = false);
        Task<HttpResponseMessage?> GetSdImage(string uri);
        Task<T?> GetApiResponse<T>(APIMethod method, string uri, object? classObject = null, CancellationToken cancellationToken = default);
    }
}