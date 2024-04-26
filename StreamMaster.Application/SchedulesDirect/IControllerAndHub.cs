using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect
{
    public interface ISchedulesDirectController
    {        
        Task<ActionResult<List<StationChannelName>>> GetStationChannelNames();
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISchedulesDirectHub
    {
        Task<List<StationChannelName>> GetStationChannelNames();
    }
}
