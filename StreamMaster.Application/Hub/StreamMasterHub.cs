using System.Runtime.CompilerServices;

using Microsoft.AspNetCore.Authorization;

namespace StreamMaster.Application.Hubs;

[Authorize(Policy = "SignalR")]
public partial class StreamMasterHub(ISender Sender, IAPIStatsLogger APIStatsLogger, IOptionsMonitor<Setting> settings, ILogger<StreamMasterHub> logger)
    : Hub<IStreamMasterHub>
{
    [BuilderIgnore]
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }
    private async Task<T> DebugAPI<T>(Task<T> task, [CallerMemberName] string callerName = "")
    {
        return await DebugAPIHelper.DebugAPI(task, logger, settings.CurrentValue.DebugAPI, callerName);
    }
}