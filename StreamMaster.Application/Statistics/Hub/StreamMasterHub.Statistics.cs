using StreamMaster.Application.Statistics;
using StreamMaster.Application.Statistics.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IStatisticsub
{

    public async Task<List<InputStreamingStatistics>> GetInputStatistics()
    {
        List<InputStreamingStatistics> res = await Sender.Send(new GetInputStatistics()).ConfigureAwait(false);
        return res;
    }

    public async Task<List<ClientStreamingStatistics>> GetClientStatistics()
    {
        List<ClientStreamingStatistics> res = await Sender.Send(new GetClientStreamingStatistics()).ConfigureAwait(false);
        return res;
    }
}