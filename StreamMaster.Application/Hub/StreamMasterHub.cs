using Microsoft.AspNetCore.Authorization;

namespace StreamMaster.Application.Hubs;

[Authorize(Policy = "SignalR")]
public partial class StreamMasterHub(ISender Sender, IAPIStatsLogger APIStatsLogger, IOptionsMonitor<Setting> settings)
    : Hub<IStreamMasterHub>;