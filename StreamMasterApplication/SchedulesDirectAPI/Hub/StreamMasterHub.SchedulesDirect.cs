using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ISchedulesDirectHub
{
    public async Task<PagedResponse<StationChannelName>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters)
    {
        return await mediator.Send(new GetPagedStationChannelNameSelections(Parameters)).ConfigureAwait(false);
    }

    public async Task<StationChannelName?> GetStationChannelNameFromDisplayName(string DisplayName)
    {
        return await mediator.Send(new GetStationChannelNameFromDisplayName(DisplayName)).ConfigureAwait(false);
    }

    public async Task<List<StationChannelName>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters)
    {
        return await mediator.Send(new GetStationChannelNamesSimpleQuery(Parameters)).ConfigureAwait(false);
    }

    public async Task<UserStatus> GetStatus()
    {
        return await mediator.Send(new GetStatus()).ConfigureAwait(false);
    }
      
}