﻿using StreamMasterApplication.Common.Models;
using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;
using StreamMasterApplication.StreamGroups.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IStreamGroupHub
{
    public async Task<StreamGroupDto?> AddStreamGroup(AddStreamGroupRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<int?> DeleteStreamGroup(DeleteStreamGroupRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
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

    public async Task<EPGGuide> GetStreamGroupEPGForGuide(int StreamGroupNumber)
    {
        return await _mediator.Send(new GetStreamGroupEPGForGuide(StreamGroupNumber)).ConfigureAwait(false);
    }

    public async Task<IEnumerable<StreamGroupDto>> GetStreamGroups()
    {
        return await _mediator.Send(new GetStreamGroups()).ConfigureAwait(false);
    }

    public async Task SimulateStreamFailure(string streamUrl)
    {
        await _mediator.Send(new SimulateStreamFailureRequest(streamUrl)).ConfigureAwait(false);
    }

    public async Task<StreamGroupDto?> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
