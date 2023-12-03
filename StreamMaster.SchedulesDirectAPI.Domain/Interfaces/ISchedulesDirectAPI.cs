using StreamMaster.SchedulesDirectAPI.Domain.Enums;

namespace StreamMaster.SchedulesDirectAPI.Domain.Interfaces
{
    public interface ISchedulesDirectAPI
    {
        Task<T?> GetApiResponse<T>(APIMethod method, string uri, object? classObject = null, CancellationToken cancellationToken = default);

    }
}