using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository.EPG;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : IProgrammeChannelHub
{
    [BuilderIgnore]
    public async Task<IEnumerable<Programme>?> GetProgramme(string Channel)
    {
        return await mediator.Send(new GetProgramme(Channel)).ConfigureAwait(false);
    }

    [BuilderIgnore]
    public async Task<IEnumerable<ProgrammeChannel>> GetProgrammeChannels()
    {
        return await mediator.Send(new GetProgrammeChannels()).ConfigureAwait(false);
    }


    public async Task<PagedResponse<ProgrammeNameDto>> GetProgrammeNameSelections(ProgrammeParameters Parameters)
    {
        return await mediator.Send(new GetProgrammeNameSelections(Parameters)).ConfigureAwait(false);
    }

    [BuilderIgnore]
    public async Task<IEnumerable<Programme>> GetProgrammes()
    {
        return await mediator.Send(new GetProgrammes()).ConfigureAwait(false);
    }

    public async Task<IEnumerable<string>> GetProgrammeNames()
    {
        return await mediator.Send(new GetProgrammeNames()).ConfigureAwait(false);
    }

    public async Task<List<ProgrammeNameDto>> GetProgrammsSimpleQuery(ProgrammeParameters Parameters)
    {
        return await mediator.Send(new GetProgrammsSimpleQuery(Parameters)).ConfigureAwait(false);
    }

    public async Task<ProgrammeNameDto?> GetProgrammeFromDisplayName(GetProgrammeFromDisplayNameRequest request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }


}
