using MediatR;

using Microsoft.AspNetCore.SignalR;

using StreamMasterApplication.General.Queries;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : Hub<IStreamMasterHub>, ISharedHub
{
    private readonly ISender _mediator = null!;

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
        _ = Clients.Caller.SystemStatusUpdate(_mediator.Send(new GetSystemStatus()).Result);
        // Add your own code here. For example: in a chat application, record
        // the association between the current connection ID and user name, and
        // mark the user as online. After the code in this method completes, the
        // client is informed that the connection is established; for example,
        // in a JavaScript client, the start().done callback is executed.
        return base.OnConnectedAsync();
    }
}
