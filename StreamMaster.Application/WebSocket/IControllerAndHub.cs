using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.WebSocket.Commands;

namespace StreamMaster.Application.WebSocket
{
    public interface IWebSocketController
    {
        Task<ActionResult<APIResponse?>> TriggerReload();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IWebSocketHub
    {
        Task<APIResponse?> TriggerReload();
    }
}
