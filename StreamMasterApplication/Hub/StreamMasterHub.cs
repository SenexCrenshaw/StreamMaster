﻿using StreamMasterApplication.General.Queries;

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

public partial class StreamMasterHub(ISender mediator, ISettingsService settingsService) : Hub<IStreamMasterHub>, ISharedHub
{
    private static readonly HashSet<string> _connections = new();

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



    public Task<bool> GetIsSystemReady()
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