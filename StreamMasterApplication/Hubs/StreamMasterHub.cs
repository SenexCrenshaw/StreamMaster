using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.General.Queries;

using StreamMasterDomain.Attributes;

namespace StreamMasterApplication.Hubs;

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

public partial class StreamMasterHub : Hub<IStreamMasterHub>, ISharedHub
{
    private readonly ISender _mediator = null!;

    private static HashSet<string> _connections = new HashSet<string>();

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

    public StreamMasterHub(
        ISender mediator
        )
    {
        _mediator = mediator;
    }

    public Task<bool> GetIsSystemReady()
    {
        return _mediator.Send(new GetIsSystemReadyRequest());
    }

    public override Task OnConnectedAsync()
    {
        lock (_connections)
        {
            _connections.Add(Context.ConnectionId);
        }

        Clients.Caller.SystemStatusUpdate(_mediator.Send(new GetSystemStatus()).Result);

        return base.OnConnectedAsync();
    }
}