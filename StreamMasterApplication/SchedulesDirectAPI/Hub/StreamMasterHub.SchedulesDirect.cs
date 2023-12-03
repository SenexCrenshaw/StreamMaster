using StreamMasterApplication.SchedulesDirectAPI;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

namespace StreamMasterApplication.Hubs;

public partial class StreamMasterHub : ISchedulesDirectHub
{   
    public async Task<UserStatus> GetStatus()
    {
        return await mediator.Send(new GetStatus()).ConfigureAwait(false);
    }
      
}