using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect
{
    public interface ISchedulesDirectController
    {        
        Task<ActionResult<List<CountryData>>> GetAvailableCountries();
        Task<ActionResult<List<HeadendDto>>> GetHeadendsByCountryPostal(GetHeadendsByCountryPostalRequest request);
        Task<ActionResult<List<HeadendToView>>> GetHeadendsToView();
        Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel(GetLineupPreviewChannelRequest request);
        Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds();
        Task<ActionResult<List<StationChannelName>>> GetStationChannelNames();
        Task<ActionResult<List<StationPreview>>> GetStationPreviews();
        Task<ActionResult<List<HeadendDto>>> GetSubScribedHeadends();
        Task<ActionResult<List<SubscribedLineup>>> GetSubscribedLineups();
        Task<ActionResult<APIResponse>> AddHeadendToView(AddHeadendToViewRequest request);
        Task<ActionResult<APIResponse>> AddLineup(AddLineupRequest request);
        Task<ActionResult<APIResponse>> AddStation(AddStationRequest request);
        Task<ActionResult<APIResponse>> RemoveHeadendToView(RemoveHeadendToViewRequest request);
        Task<ActionResult<APIResponse>> RemoveLineup(RemoveLineupRequest request);
        Task<ActionResult<APIResponse>> RemoveStation(RemoveStationRequest request);
        Task<ActionResult<APIResponse>> SetStations(SetStationsRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISchedulesDirectHub
    {
        Task<List<CountryData>> GetAvailableCountries();
        Task<List<HeadendDto>> GetHeadendsByCountryPostal(GetHeadendsByCountryPostalRequest request);
        Task<List<HeadendToView>> GetHeadendsToView();
        Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(GetLineupPreviewChannelRequest request);
        Task<List<StationIdLineup>> GetSelectedStationIds();
        Task<List<StationChannelName>> GetStationChannelNames();
        Task<List<StationPreview>> GetStationPreviews();
        Task<List<HeadendDto>> GetSubScribedHeadends();
        Task<List<SubscribedLineup>> GetSubscribedLineups();
        Task<APIResponse> AddHeadendToView(AddHeadendToViewRequest request);
        Task<APIResponse> AddLineup(AddLineupRequest request);
        Task<APIResponse> AddStation(AddStationRequest request);
        Task<APIResponse> RemoveHeadendToView(RemoveHeadendToViewRequest request);
        Task<APIResponse> RemoveLineup(RemoveLineupRequest request);
        Task<APIResponse> RemoveStation(RemoveStationRequest request);
        Task<APIResponse> SetStations(SetStationsRequest request);
    }
}
