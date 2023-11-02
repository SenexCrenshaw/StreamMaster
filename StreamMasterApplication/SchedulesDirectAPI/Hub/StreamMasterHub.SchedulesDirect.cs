
using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI;
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

    public async Task<LineUpResult?> GetLineup(string lineup)
    {
        return await mediator.Send(new GetLineup(lineup)).ConfigureAwait(false);
    }

    public async Task<List<LineUpPreview>> GetLineupPreviews()
    {
        return await mediator.Send(new GetLineupPreviews()).ConfigureAwait(false);
    }

    public async Task<List<Lineup>> GetLineups()
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

    public async Task<List<StationIdLineUp>> GetSelectedStationIds()
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
}
