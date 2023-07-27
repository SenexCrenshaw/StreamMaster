using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Attributes;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities.EPG;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IProgrammeChannelHub
{
    [BuilderIgnore]
    public async Task<IEnumerable<Programme>?> GetProgramme(string Channel)
    {
        return await _mediator.Send(new GetProgramme(Channel)).ConfigureAwait(false);
    }

    [BuilderIgnore]
    public async Task<IEnumerable<ProgrammeChannel>> GetProgrammeChannels()
    {
        return await _mediator.Send(new GetProgrammeChannels()).ConfigureAwait(false);
    }

    [JustUpdates]
    public async Task<IEnumerable<ProgrammeNameDto>> GetProgrammeNames()
    {
        return await _mediator.Send(new GetProgrammeNames()).ConfigureAwait(false);
    }

    [BuilderIgnore]
    public async Task<IEnumerable<Programme>> GetProgrammes()
    {
        return await _mediator.Send(new GetProgrammes()).ConfigureAwait(false);
    }
}
