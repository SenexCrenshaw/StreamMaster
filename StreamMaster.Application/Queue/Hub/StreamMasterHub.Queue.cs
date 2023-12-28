using StreamMaster.Application.Common.Models;
using StreamMaster.Application.Queue;
using StreamMaster.Application.Queue.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IQueueHub
{
    public async Task<List<TaskQueueStatus>> GetQueueStatus()
    {
        return await mediator.Send(new GetQueueStatus()).ConfigureAwait(false);
    }

}
