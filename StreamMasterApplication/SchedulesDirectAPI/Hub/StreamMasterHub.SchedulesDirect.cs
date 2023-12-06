using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ISchedulesDirectHub
{
    public async Task<bool> AddLineup(AddLineup request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> AddStation(AddStation request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<List<CountryData>?> GetAvailableCountries()
    {
        return await mediator.Send(new GetAvailableCountries()).ConfigureAwait(false);
    }

    public async Task<List<string>> GetChannelNames()
    {
        return await mediator.Send(new GetChannelNames()).ConfigureAwait(false);
    }

    public async Task<List<HeadendDto>> GetHeadends(string country, string postalCode)
    {
        return await mediator.Send(new GetHeadends(country, postalCode)).ConfigureAwait(false);
    }

    public async Task<List<SubscribedLineup>> GetLineups()
    {
        return await mediator.Send(new GetLineups()).ConfigureAwait(false);
    }

    public async Task<PagedResponse<StationChannelName>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters)
    {
        return await mediator.Send(new GetPagedStationChannelNameSelections(Parameters)).ConfigureAwait(false);
    }

    public async Task<List<StationIdLineup>> GetSelectedStationIds()
    {
        return await mediator.Send(new GetSelectedStationIds()).ConfigureAwait(false);
    }

    public async Task<List<StationChannelMap>> GetStationChannelMaps()
    {
        return await mediator.Send(new GetStationChannelMaps()).ConfigureAwait(false);
    }

    public async Task<StationChannelName?> GetStationChannelNameFromDisplayName(GetStationChannelNameFromDisplayName request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<List<StationChannelName>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters)
    {
        return await mediator.Send(new GetStationChannelNamesSimpleQuery(Parameters)).ConfigureAwait(false);
    }

    public async Task<List<StationPreview>> GetStationPreviews()
    {
        return await mediator.Send(new GetStationPreviews()).ConfigureAwait(false);
    }

    public async Task<UserStatus> GetStatus()
    {
        return await mediator.Send(new GetStatus()).ConfigureAwait(false);
    }

    public async Task<bool> RemoveLineup(RemoveLineup request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> RemoveStation(RemoveStation request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }
}