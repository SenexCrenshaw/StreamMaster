using StreamMaster.Application.Queue;
using StreamMaster.Application.Queue.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IQueueHub
{
    public async Task<List<SMTask>> GetQueueStatus()
    {
        return await Sender.Send(new GetQueueStatus()).ConfigureAwait(false);
    }

}
