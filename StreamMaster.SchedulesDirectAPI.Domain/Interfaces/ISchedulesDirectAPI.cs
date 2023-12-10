using StreamMaster.SchedulesDirectAPI.Domain.Enums;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface ISchedulesDirectAPI
    {
        bool CheckToken(bool forceReset = false);
        Task<HttpResponseMessage> GetSdImage(string uri);
        Task<T?> GetApiResponse<T>(APIMethod method, string uri, object? classObject = null, CancellationToken cancellationToken = default);
    }
}