using StreamMasterApplication.StreamGroupVideoStreams;
using StreamMasterApplication.StreamGroupVideoStreams.Commands;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IStreamGroupVideoStreamHub
{
    //public async Task AddVideoStreamToStreamGroup(AddVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    //{
    //    await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    //}

    //public async Task RemoveVideoStreamFromStreamGroup(RemoveVideoStreamFromStreamGroupRequest request, CancellationToken cancellationToken)
    //{
    //    await _mediator.Send(request, cancellationToken).ConfigureAwait(false);
    //}

    public async Task SetVideoStreamRanks(SetVideoStreamRanksRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task SyncVideoStreamToStreamGroup(SyncVideoStreamToStreamGroupRequest request, CancellationToken cancellationToken)
    {
        await mediator.Send(request, cancellationToken).ConfigureAwait(false);
    }
}