using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI;
using StreamMaster.SchedulesDirectAPI.Models;
using StreamMasterDomain.Dto;

namespace StreamMasterApplication.SchedulesDirectAPI;

public interface ISchedulesDirectController
{
    Task<ActionResult<Countries?>> GetCountries();

    Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode);

    Task<ActionResult<LineUpResult?>> GetLineup(string lineup);

    Task<ActionResult<List<LineUpPreview>>> GetLineupPreviews();

    Task<ActionResult<LineUpsResult?>> GetLineups();

    Task<ActionResult<List<Schedule>?>> GetSchedules();

    Task<ActionResult<List<StationPreview>>> GetStationPreviews();

    Task<ActionResult<List<Station>>> GetStations();

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

    Task<List<LineUpPreview>> GetLineupPreviews();

    Task<LineUpsResult?> GetLineups();

    Task<List<Schedule>?> GetSchedules();

    Task<List<StationPreview>> GetStationPreviews();

    Task<List<Station>> GetStations();

    Task<SDStatus> GetStatus();
}

public interface ISchedulesDirectTasks
{
}
