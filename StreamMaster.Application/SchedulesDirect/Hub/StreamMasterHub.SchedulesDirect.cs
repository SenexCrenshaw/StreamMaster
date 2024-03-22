using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.SchedulesDirect;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.SchedulesDirect.Queries;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : ISchedulesDirectHub
{
    public async Task<bool> AddLineup(AddLineup request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> AddStation(AddStation request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<List<CountryData>?> GetAvailableCountries()
    {
        return await Sender.Send(new GetAvailableCountries()).ConfigureAwait(false);
    }

    public async Task<List<string>> GetChannelNames()
    {
        return await Sender.Send(new GetChannelNames()).ConfigureAwait(false);
    }

    public async Task<List<HeadendDto>> GetHeadends(GetHeadends request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(GetLineupPreviewChannel request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<List<SubscribedLineup>> GetLineups()
    {
        return await Sender.Send(new GetLineups()).ConfigureAwait(false);
    }

    public async Task<PagedResponse<StationChannelName>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters)
    {
        return await Sender.Send(new GetPagedStationChannelNameSelections(Parameters)).ConfigureAwait(false);
    }

    public async Task<List<StationIdLineup>> GetSelectedStationIds()
    {
        return await Sender.Send(new GetSelectedStationIds()).ConfigureAwait(false);
    }

    public async Task<List<StationChannelMap>> GetStationChannelMaps()
    {
        return await Sender.Send(new GetStationChannelMaps()).ConfigureAwait(false);
    }

    public async Task<StationChannelName?> GetStationChannelNameFromDisplayName(GetStationChannelNameFromDisplayName request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<List<StationChannelName>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters)
    {
        return await Sender.Send(new GetStationChannelNamesSimpleQuery(Parameters)).ConfigureAwait(false);
    }

    public async Task<List<StationChannelName>> GetStationChannelNames()
    {
        return await Sender.Send(new GetStationChannelNames()).ConfigureAwait(false);
    }

    public async Task<List<StationPreview>> GetStationPreviews()
    {
        return await Sender.Send(new GetStationPreviews()).ConfigureAwait(false);
    }

    public async Task<UserStatus> GetUserStatus()
    {
        return await Sender.Send(new GetUserStatus()).ConfigureAwait(false);
    }

    public async Task<bool> RemoveLineup(RemoveLineup request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> RemoveStation(RemoveStation request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }
}