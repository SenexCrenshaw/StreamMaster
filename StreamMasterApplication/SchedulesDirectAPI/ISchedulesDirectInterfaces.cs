using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;

namespace StreamMasterApplication.SchedulesDirectAPI;

public interface ISchedulesDirectController
{
    Task<IActionResult> GetEpg();
    Task<ActionResult<Countries?>> GetCountries();
    Task<ActionResult<List<StationIdLineUp>>> GetSelectedStationIds();
    Task<ActionResult<List<SDProgram>>> GetSDPrograms();
    Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode);

    Task<ActionResult<LineUpResult?>> GetLineup(string lineup);

    Task<ActionResult<List<LineUpPreview>>> GetLineupPreviews();

    Task<ActionResult<LineUpsResult?>> GetLineups();

    Task<ActionResult<List<Schedule>>> GetSchedules();

    Task<ActionResult<List<StationPreview>>> GetStationPreviews();

    Task<ActionResult<List<Station>>> GetStations();

    Task<ActionResult<SDStatus>> GetStatus();
}

public interface ISchedulesDirectDB
{
}

public interface ISchedulesDirectHub
{
    Task<string> GetEpg();
    Task<List<StationIdLineUp>> GetSelectedStationIds();
    Task<Countries> GetCountries();
    Task<List<SDProgram>> GetSDPrograms();
    Task<List<HeadendDto>> GetHeadends(string country, string postalCode);

    Task<LineUpResult> GetLineup(string lineup);

    Task<List<LineUpPreview>> GetLineupPreviews();

    Task<LineUpsResult> GetLineups();

    Task<List<Schedule>> GetSchedules();

    Task<List<StationPreview>> GetStationPreviews();

    Task<List<Station>> GetStations();

    Task<SDStatus> GetStatus();
}

public interface ISchedulesDirectTasks
{
}
