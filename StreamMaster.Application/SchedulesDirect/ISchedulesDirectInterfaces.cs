using Microsoft.AspNetCore.Mvc;

using StreamMaster.Domain.Dto;
using StreamMaster.Domain.Pagination;

using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect;

public interface ISchedulesDirectController
{
    Task<ActionResult<bool>> AddLineup(AddLineup request);
    Task<ActionResult<bool>> AddStation(AddStation request);

    Task<ActionResult<bool>> RemoveStation(RemoveStation request);
    Task<ActionResult<bool>> RemoveLineup(RemoveLineup request);

    Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel(GetLineupPreviewChannel request);
    //Task<ActionResult<List<string>>> GetLineupNames();

    //Task<ActionResult<Country?>> GetCountry();

    Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds();

    //Task<ActionResult<List<Programme>>> GetSDPrograms();

    Task<ActionResult<List<HeadendDto>>> GetHeadends(GetHeadends request);
    Task<ActionResult<List<SubscribedLineup>>> GetLineups();


    //Task<ActionResult<LineupPreviewChannel?>> GetLineup(string lineup);

    //Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannels();

    //Task<ActionResult<List<Schedule>>> GetSchedules();

    Task<ActionResult<List<StationPreview>>> GetStationPreviews();

    Task<ActionResult<List<StationChannelName>>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters);
    Task<ActionResult<PagedResponse<StationChannelName>>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters);
    Task<ActionResult<StationChannelName>> GetStationChannelNameFromDisplayName(GetStationChannelNameFromDisplayName request);
    Task<ActionResult<List<CountryData>?>> GetAvailableCountries();
    Task<ActionResult<List<string>>> GetChannelNames();
    Task<ActionResult<UserStatus>> GetUserStatus();
    Task<ActionResult<List<StationChannelMap>>> GetStationChannelMaps();

}

public interface ISchedulesDirectHub
{
    Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(GetLineupPreviewChannel request);
    Task<bool> AddLineup(AddLineup request);
    Task<bool> AddStation(AddStation request);

    Task<bool> RemoveStation(RemoveStation request);
    Task<bool> RemoveLineup(RemoveLineup request);


    Task<List<StationChannelMap>> GetStationChannelMaps();
    Task<List<SubscribedLineup>> GetLineups();

    //Task<List<string>> GetLineupNames();

    Task<List<StationIdLineup>> GetSelectedStationIds();

    //Task<Country> GetCountry();

    //Task<List<Programme>> GetSDPrograms();

    Task<List<HeadendDto>> GetHeadends(GetHeadends request);

    //Task<LineupPreviewChannel> GetLineup(string lineup);

    //   Task<List<LineupPreviewChannel>> GetLineupPreviewChannels();

    //Task<List<Schedule>> GetSchedules();

    Task<List<StationPreview>> GetStationPreviews();

    //Task<List<Station>> GetStations();

    Task<List<CountryData>?> GetAvailableCountries();

    Task<List<string>> GetChannelNames();
    Task<List<StationChannelName>> GetStationChannelNamesSimpleQuery([FromQuery] StationChannelNameParameters Parameters);
    Task<PagedResponse<StationChannelName>> GetPagedStationChannelNameSelections([FromQuery] StationChannelNameParameters Parameters);
    Task<StationChannelName?> GetStationChannelNameFromDisplayName(GetStationChannelNameFromDisplayName request);

    Task<UserStatus> GetUserStatus();
}
