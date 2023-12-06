using StreamMaster.SchedulesDirectAPI.Domain.XmltvXml;

using StreamMasterApplication.Programmes;
using StreamMasterApplication.Programmes.Queries;

namespace StreamMasterApplication.Hubs;

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
