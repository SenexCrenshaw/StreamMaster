using StreamMaster.Domain.Dto;

using StreamMaster.Application.LogApp;
using StreamMaster.Application.LogApp.Queries;

namespace StreamMaster.Application.Hubs;

public partial class StreamMasterHub : ILogHub
{
    public async Task<IEnumerable<LogEntryDto>> GetLog(GetLogRequest request)
    {
        return await Sender.Send(request).ConfigureAwait(false);
    }
}
