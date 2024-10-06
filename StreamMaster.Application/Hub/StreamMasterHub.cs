namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub(ISender Sender, IOptionsMonitor<Setting> intSettings)
    : Hub<IStreamMasterHub>
{
    private static readonly ConcurrentHashSet<string> _connections = [];
    private readonly Setting settings = intSettings.CurrentValue;
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
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        lock (_connections)
        {
            _connections.Remove(Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public override Task OnConnectedAsync()
    {
        lock (_connections)
        {
            _connections.Add(Context.ConnectionId);
        }

        //ClientChannels.Caller.SystemStatusUpdate(Sender.Send(new GetSystemStatusRequest()).Result.Data);

        return base.OnConnectedAsync();
    }
}