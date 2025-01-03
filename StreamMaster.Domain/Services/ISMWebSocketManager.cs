
using Microsoft.AspNetCore.Http;

namespace StreamMaster.Domain.Services
{
    public interface ISMWebSocketManager
    {
        Task BroadcastReloadAsync();
        Task HandleWebSocketAsync(HttpContext context);
    }
}