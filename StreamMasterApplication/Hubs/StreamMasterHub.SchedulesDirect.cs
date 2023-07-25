using StreamMaster.SchedulesDirectAPI;

using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ISchedulesDirectHub
{
    public async Task<Countries?> GetCountries()
    {
        return await _mediator.Send(new GetCountries()).ConfigureAwait(false);
    }

    public async Task<List<HeadendDto>> GetHeadends(string country, string postalCode)
    {
        return await _mediator.Send(new GetHeadends(country, postalCode)).ConfigureAwait(false);
    }

    public async Task<LineUpResult?> GetLineup(string lineup)
    {
        return await _mediator.Send(new GetLineup(lineup)).ConfigureAwait(false);
    }

    public async Task<List<LineUpPreview>> GetLineupPreviews()
    {
        return await _mediator.Send(new GetLineupPreviews()).ConfigureAwait(false);
    }

    public async Task<LineUpsResult?> GetLineups()
    {
        return await _mediator.Send(new GetLineups()).ConfigureAwait(false);
    }

    public async Task<List<Schedule>?> GetSchedules()
    {
        return await _mediator.Send(new GetSchedules()).ConfigureAwait(false);
    }

    public async Task<List<StationPreview>> GetStationPreviews()
    {
        return await _mediator.Send(new GetStationPreviewsRequest()).ConfigureAwait(false);
    }

    public async Task<List<Station>> GetStations()
    {
        return await _mediator.Send(new GetStations()).ConfigureAwait(false);
    }

    public async Task<SDStatus> GetStatus()
    {
        return await _mediator.Send(new GetStatus()).ConfigureAwait(false);
    }
}
