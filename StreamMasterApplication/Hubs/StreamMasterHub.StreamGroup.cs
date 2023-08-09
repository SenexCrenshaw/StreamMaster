using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IStreamGroupHub
{
    public async Task AddStreamGroup(AddStreamGroupRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task FailClient(FailClientRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }

    [JustUpdates]
    public async Task<List<StreamStatisticsResult>> GetAllStatisticsForAllUrls()
    {
        return await _mediator.Send(new GetAllStatisticsForAllUrls()).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> GetStreamGroup(int id)
    {
        return await _mediator.Send(new GetStreamGroup(id)).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> GetStreamGroupByStreamNumber(int StreamGroupNumber)
    {
        return await _mediator.Send(new GetStreamGroup(StreamGroupNumber)).ConfigureAwait(false);
    }

    public async Task<EPGGuide> GetStreamGroupEPGForGuide(int StreamGroupNumber)
    {
        return await _mediator.Send(new GetStreamGroupEPGForGuide(StreamGroupNumber)).ConfigureAwait(false);
    }

    public async Task<IPagedList<StreamGroupDto>> GetStreamGroups(StreamGroupParameters streamGroupParameters)
    {
        return await _mediator.Send(new GetStreamGroups(streamGroupParameters)).ConfigureAwait(false);
    }

    public async Task SimulateStreamFailure(string streamUrl)
    {
        await _mediator.Send(new SimulateStreamFailureRequest(streamUrl)).ConfigureAwait(false);
    }

    public async Task UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        await _mediator.Send(request).ConfigureAwait(false);
    }
}