using StreamMaster.Application.General.Queries;
using StreamMaster.Application.Services;
using StreamMaster.Domain.Configuration;

namespace StreamMaster.Application.Hubs;

public enum ModelAction
{
    Unknown = 0,
    Created = 1,
    Updated = 2,
    Deleted = 3,
    Sync = 4
}

public class SignalRMessage
{
    public object Body { get; set; }
    public string Name { get; set; }

    [System.Text.Json.Serialization.JsonIgnore]
    public ModelAction Action { get; set; }
}

public partial class StreamMasterHub(ISender mediator, IBackgroundTaskQueue taskQueue, IOptionsMonitor<Setting> intsettings) : Hub<IStreamMasterHub>, ISharedHub
{
    private static readonly ConcurrentHashSet<string> _connections = [];
    private readonly Setting settings = intsettings.CurrentValue;

    public static bool IsConnected
    {
        get
        {
            lock (_connections)
            {
                return _connections.Count != 0;
            }
        }
    }

    [BuilderIgnore]
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        lock (_connections)
        {
            _connections.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }



    public Task<bool> GetIsSystemReady(object waste)
    {
        return mediator.Send(new GetIsSystemReadyRequest());
    }

    public override Task OnConnectedAsync()
    {
        lock (_connections)
        {
            _connections.Add(Context.ConnectionId);
        }

        Clients.Caller.SystemStatusUpdate(mediator.Send(new GetSystemStatus()).Result);

        return base.OnConnectedAsync();
    }
}