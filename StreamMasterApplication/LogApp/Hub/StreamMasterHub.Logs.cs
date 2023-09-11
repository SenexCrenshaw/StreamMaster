using StreamMasterApplication.LogApp;
using StreamMasterApplication.LogApp.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ILogHub
{
    public async Task<IEnumerable<LogEntryDto>> GetLogRequest(GetLog request)
    {
        return await mediator.Send(request).ConfigureAwait(false);
    }
}
