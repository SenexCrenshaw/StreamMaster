using StreamMaster.Application.Statistics.Queries;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Application.StreamGroups.CommandsOld;
using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Domain.Requests;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IStreamGroupHub
{
    public async Task CreateStreamGroup(CreateStreamGroupRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }


    public async Task DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task FailClient(FailClientRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    [JustUpdates]
    public async Task<List<ClientStreamingStatistics>> GetAllStatisticsForAllUrls()
    {
        return await Sender.Send(new GetClientStreamingStatistics()).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> GetStreamGroup(int id)
    {
        return await Sender.Send(new GetStreamGroup(id)).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> GetStreamGroupByStreamNumber(int StreamGroupNumber)
    {
        return await Sender.Send(new GetStreamGroup(StreamGroupNumber)).ConfigureAwait(false);
    }


    public async Task SimulateStreamFailure(SimulateStreamFailureRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<string?> GetStreamGroupVideoStreamUrl(string VideoStreamId)
    {
        string? res = await Sender.Send(new GetStreamGroupVideoStreamUrl(VideoStreamId)).ConfigureAwait(false);
        return res;
    }

}