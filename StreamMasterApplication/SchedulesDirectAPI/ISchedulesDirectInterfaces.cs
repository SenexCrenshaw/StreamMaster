using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI;

using StreamMasterDomain.Dto;

namespace StreamMasterApplication.SchedulesDirectAPI;

public interface ISchedulesDirectController
{
    Task<ActionResult<Countries?>> GetCountries();

    Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode);

    Task<ActionResult<LineUpResult?>> GetLineup(string lineup);

    Task<ActionResult<LineUpsResult?>> GetLineups();

    Task<ActionResult<List<Schedule>?>> GetSchedules(List<string> stationIds);

    Task<ActionResult<SDStatus>> GetStatus();
}

public interface ISchedulesDirectDB
{
}

public interface ISchedulesDirectHub
{
    Task<Countries?> GetCountries();

    Task<List<HeadendDto>> GetHeadends(string country, string postalCode);

    Task<LineUpResult?> GetLineup(string lineup);

    Task<LineUpsResult?> GetLineups();

    Task<List<Schedule>?> GetSchedules(List<string> stationIds);

    Task<SDStatus> GetStatus();
}

public interface ISchedulesDirectTasks
{
}
