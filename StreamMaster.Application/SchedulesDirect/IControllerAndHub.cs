using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect
{
    public interface ISchedulesDirectController
    {        
        Task<ActionResult<List<CountryData>>> GetAvailableCountries();
        Task<ActionResult<List<string>>> GetChannelNames();
        Task<ActionResult<List<HeadendDto>>> GetHeadends(GetHeadendsRequest request);
        Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel(GetLineupPreviewChannelRequest request);
        Task<ActionResult<List<SubscribedLineup>>> GetLineups();
        Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds();
        Task<ActionResult<MxfService>> GetService(GetServiceRequest request);
        Task<ActionResult<List<StationChannelMap>>> GetStationChannelMaps();
        Task<ActionResult<List<StationChannelName>>> GetStationChannelNames();
        Task<ActionResult<List<StationPreview>>> GetStationPreviews();
        Task<ActionResult<UserStatus>> GetUserStatus();
        Task<ActionResult<APIResponse>> AddLineup(AddLineupRequest request);
        Task<ActionResult<APIResponse>> AddStation(AddStationRequest request);
        Task<ActionResult<APIResponse>> EPGSync();
        Task<ActionResult<APIResponse>> RemoveLineup(RemoveLineupRequest request);
        Task<ActionResult<APIResponse>> RemoveStation(RemoveStationRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface ISchedulesDirectHub
    {
        Task<List<CountryData>> GetAvailableCountries();
        Task<List<string>> GetChannelNames();
        Task<List<HeadendDto>> GetHeadends(GetHeadendsRequest request);
        Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(GetLineupPreviewChannelRequest request);
        Task<List<SubscribedLineup>> GetLineups();
        Task<List<StationIdLineup>> GetSelectedStationIds();
        Task<MxfService> GetService(GetServiceRequest request);
        Task<List<StationChannelMap>> GetStationChannelMaps();
        Task<List<StationChannelName>> GetStationChannelNames();
        Task<List<StationPreview>> GetStationPreviews();
        Task<UserStatus> GetUserStatus();
        Task<APIResponse> AddLineup(AddLineupRequest request);
        Task<APIResponse> AddStation(AddStationRequest request);
        Task<APIResponse> EPGSync();
        Task<APIResponse> RemoveLineup(RemoveLineupRequest request);
        Task<APIResponse> RemoveStation(RemoveStationRequest request);
    }
}
