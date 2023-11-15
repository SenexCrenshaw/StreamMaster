using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI.Commands;

namespace StreamMasterApplication.SchedulesDirectAPI;

public interface ISchedulesDirectController
{
    Task<ActionResult<bool>> AddLineup(AddLineup request);

    Task<ActionResult<bool>> RemoveLineup(RemoveLineup request);

    Task<ActionResult<List<string>>> GetLineupNames();

    Task<ActionResult> GetEpg();

    Task<ActionResult<Countries?>> GetCountries();

    Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds();

    Task<ActionResult<List<SDProgram>>> GetSDPrograms();

    Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode);

    Task<ActionResult<LineupResult?>> GetLineup(string lineup);

    Task<ActionResult<List<LineupPreview>>> GetLineupPreviews();

    Task<ActionResult<List<StreamMaster.SchedulesDirectAPI.Domain.Models.Lineup>>> GetLineups();

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
    Task<bool> AddLineup(AddLineup request);

    Task<bool> RemoveLineup(RemoveLineup request);

    Task<List<string>> GetLineupNames();

    Task<string> GetEpg();

    Task<List<StationIdLineup>> GetSelectedStationIds();

    Task<Countries> GetCountries();

    Task<List<SDProgram>> GetSDPrograms();

    Task<List<HeadendDto>> GetHeadends(string country, string postalCode);

    Task<LineupResult> GetLineup(string lineup);

    Task<List<LineupPreview>> GetLineupPreviews();

    Task<List<StreamMaster.SchedulesDirectAPI.Domain.Models.Lineup>> GetLineups();

    Task<List<Schedule>> GetSchedules();

    Task<List<StationPreview>> GetStationPreviews();

    Task<List<Station>> GetStations();

    Task<SDStatus> GetStatus();
}

public interface ISchedulesDirectTasks
{
}