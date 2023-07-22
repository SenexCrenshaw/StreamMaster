using StreamMasterApplication.Logs;
using StreamMasterApplication.Logs.Queries;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ILogHub
{
    public async Task<IEnumerable<LogEntryDto>> GetLogRequest(GetLog request)
    {
        return await _mediator.Send(request).ConfigureAwait(false);
    }
}
