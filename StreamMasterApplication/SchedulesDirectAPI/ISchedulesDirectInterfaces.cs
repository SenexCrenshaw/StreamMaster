using Microsoft.AspNetCore.Mvc;

using StreamMaster.SchedulesDirectAPI.Domain.Models;

using StreamMasterApplication.SchedulesDirectAPI.Commands;
using StreamMasterApplication.SchedulesDirectAPI.Queries;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.SchedulesDirectAPI;

public interface ISchedulesDirectController
{
    Task<ActionResult<bool>> AddLineup(AddLineup request);
    Task<ActionResult<bool>> AddStation(AddStation request);

    Task<ActionResult<bool>> RemoveStation(RemoveStation request);
    Task<ActionResult<bool>> RemoveLineup(RemoveLineup request);

    Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel(string Lineup);
    //Task<ActionResult<List<string>>> GetLineupNames();

    //Task<ActionResult<Country?>> GetCountry();

    Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds();

    //Task<ActionResult<List<Programme>>> GetSDPrograms();

    Task<ActionResult<List<HeadendDto>>> GetHeadends(string country, string postalCode);
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
    Task<ActionResult<UserStatus>> GetStatus();
    Task<ActionResult<List<StationChannelMap>>> GetStationChannelMaps();
    
}

public interface ISchedulesDirectDB
{
}

public interface ISchedulesDirectHub
{
    Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(string Lineup);
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

    Task<List<HeadendDto>> GetHeadends(string country, string postalCode);

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

    Task<UserStatus> GetStatus();
}

public interface ISchedulesDirectTasks
{
}