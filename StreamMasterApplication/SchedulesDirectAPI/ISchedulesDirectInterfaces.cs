using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.SchedulesDirectAPI;

public interface ISchedulesDirectController
{
    //Task<ActionResult<bool>> AddLineup(AddLineup request);

    //Task<ActionResult<bool>> RemoveLineup(RemoveLineup request);

    //Task<ActionResult<List<string>>> GetLineupNames();

    //Task<ActionResult<Country?>> GetCountry();

    //Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds();

    //Task<ActionResult<List<Programme>>> GetSDPrograms();

    //Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode);

    //Task<ActionResult<LineupPreviewChannel?>> GetLineup(string lineup);

    //Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannels();

    //Task<ActionResult<List<Schedule>>> GetSchedules();

    //Task<ActionResult<List<StationPreview>>> GetStationPreviews();

    Task<ActionResult<List<StationChannelName>>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters);
    Task<ActionResult<PagedResponse<StationChannelName>>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters);
    Task<ActionResult<StationChannelName>> GetStationChannelNameFromDisplayName(string DisplayName);

    Task<ActionResult<UserStatus>> GetStatus();
}

public interface ISchedulesDirectDB
{
}

public interface ISchedulesDirectHub
{
    //Task<bool> AddLineup(AddLineup request);

    //Task<bool> RemoveLineup(RemoveLineup request);

    //Task<List<string>> GetLineupNames();

    //Task<List<StationIdLineup>> GetSelectedStationIds();

    //Task<Country> GetCountry();

    //Task<List<Programme>> GetSDPrograms();

    //Task<List<HeadendDto>> GetHeadends(string country, string postalCode);

    //Task<LineupPreviewChannel> GetLineup(string lineup);

    //   Task<List<LineupPreviewChannel>> GetLineupPreviewChannels();

    //Task<List<Schedule>> GetSchedules();

    //Task<List<StationPreview>> GetStationPreviews();

    //Task<List<Station>> GetStations();

    Task<List<StationChannelName>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters);
    Task<PagedResponse<StationChannelName>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters);
    Task<StationChannelName?> GetStationChannelNameFromDisplayName(string DisplayName);

    Task<UserStatus> GetStatus();
}

public interface ISchedulesDirectTasks
{
}