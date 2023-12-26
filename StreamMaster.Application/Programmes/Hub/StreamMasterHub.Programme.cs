


using StreamMaster.Application.Programmes;
using StreamMaster.Application.Programmes.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : IProgrammeChannelHub
{
    [BuilderIgnore]
    public async Task<IEnumerable<XmltvProgramme>?> GetProgramme(string Channel)
    {
        return await mediator.Send(new GetProgramme(Channel)).ConfigureAwait(false);
    }


    [BuilderIgnore]
    public async Task<IEnumerable<XmltvProgramme>> GetProgrammes()
    {
        return await mediator.Send(new GetProgrammesRequest()).ConfigureAwait(false);
    }

}
