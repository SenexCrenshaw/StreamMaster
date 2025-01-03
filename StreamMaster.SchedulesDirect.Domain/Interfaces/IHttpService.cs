using StreamMaster.SchedulesDirect.Domain.Enums;

namespace StreamMaster.SchedulesDirect.Domain.Interfaces
{
    public interface IHttpService
    {
        bool GoodToken { get; }
        string? Token { get; }
        DateTime TokenTimestamp { get; }

        void ClearToken();
        Task<bool> RefreshTokenAsync(CancellationToken cancellationToken);
        Task<HttpResponseMessage> SendRawRequestAsync(HttpRequestMessage message, CancellationToken cancellationToken = default);
        Task<T?> SendRequestAsync<T>(APIMethod method, string endpoint, object? payload = null, CancellationToken cancellationToken = default);
        Task<bool> ValidateTokenAsync(bool forceReset = false, CancellationToken cancellationToken = default);
    }
}