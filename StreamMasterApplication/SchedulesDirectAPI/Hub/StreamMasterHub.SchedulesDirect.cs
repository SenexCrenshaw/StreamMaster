using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ISchedulesDirectHub
{
    public async Task<Countries?> GetCountries()
    {
        return await mediator.Send(new GetCountries()).ConfigureAwait(false);
    }

    public async Task<List<HeadendDto>> GetHeadends(string country, string postalCode)
    {
        return await mediator.Send(new GetHeadends(country, postalCode)).ConfigureAwait(false);
    }

    public async Task<LineupResult?> GetLineup(string lineup)
    {
        return await mediator.Send(new GetLineup(lineup)).ConfigureAwait(false);
    }

    public async Task<List<LineupPreview>> GetLineupPreviews()
    {
        return await mediator.Send(new GetLineupPreviews()).ConfigureAwait(false);
    }

    public async Task<List<StreamMaster.SchedulesDirectAPI.Domain.Models.Lineup>> GetLineups()
    {
        return await mediator.Send(new GetLineups()).ConfigureAwait(false);
    }

    public async Task<List<SDProgram>> GetSDPrograms()
    {
        return await mediator.Send(new GetSDPrograms()).ConfigureAwait(false);
    }

    public async Task<List<Schedule>> GetSchedules()
    {
        return await mediator.Send(new GetSchedules()).ConfigureAwait(false);
    }

    public async Task<List<StationIdLineup>> GetSelectedStationIds()
    {
        return await mediator.Send(new GetSelectedStationIds()).ConfigureAwait(false);
    }

    public async Task<List<StationPreview>> GetStationPreviews()
    {
        return await mediator.Send(new GetStationPreviewsRequest()).ConfigureAwait(false);
    }

    public async Task<List<Station>> GetStations()
    {
        return await mediator.Send(new GetStations()).ConfigureAwait(false);
    }

    public async Task<SDStatus> GetStatus()
    {
        return await mediator.Send(new GetStatus()).ConfigureAwait(false);
    }

    public async Task<string> GetEpg()
    {
        return await mediator.Send(new GetEpg()).ConfigureAwait(false);
    }

    public async Task<List<string>> GetLineupNames()
    {
        return await mediator.Send(new GetLineupNames()).ConfigureAwait(false);
    }

    public async Task<bool> AddLineup(AddLineup request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }

    public async Task<bool> RemoveLineup(RemoveLineup request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }
}