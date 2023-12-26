using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;
using StreamMaster.Domain.Requests;

using StreamMaster.Application.Common.Models;
using StreamMaster.Application.StreamGroups;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IStreamGroupHub
{
    public async Task CreateStreamGroup(CreateStreamGroupRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }


    public async Task DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task FailClient(FailClientRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    [JustUpdates]
    public async Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls()
    {
        return await mediator.Send(new GetAllStatisticsForAllUrls()).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> GetStreamGroup(int id)
    {
        return await mediator.Send(new GetStreamGroup(id)).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> GetStreamGroupByStreamNumber(int StreamGroupNumber)
    {
        return await mediator.Send(new GetStreamGroup(StreamGroupNumber)).ConfigureAwait(false);
    }

    public async Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(StreamGroupParameters streamGroupParameters)
    {
        return await mediator.Send(new GetPagedStreamGroups(streamGroupParameters)).ConfigureAwait(false);
    }

    public async Task SimulateStreamFailure(SimulateStreamFailureRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<string?> GetStreamGroupVideoStreamUrl(string VideoStreamId)
    {
        string? res = await mediator.Send(new GetStreamGroupVideoStreamUrl(VideoStreamId)).ConfigureAwait(false);
        return res;
    }
}