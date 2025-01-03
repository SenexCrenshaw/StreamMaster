using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect.Controllers
{
    [Authorize]
    public partial class SchedulesDirectController(ILogger<SchedulesDirectController> _logger) : ApiControllerBase, ISchedulesDirectController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CountryData>>> GetAvailableCountries()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetAvailableCountriesRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetAvailableCountries.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetAvailableCountries.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<HeadendDto>>> GetHeadendsByCountryPostal([FromQuery] GetHeadendsByCountryPostalRequest request)
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetHeadendsByCountryPostal.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetHeadendsByCountryPostal.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<HeadendToView>>> GetHeadendsToView()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetHeadendsToViewRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetHeadendsToView.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetHeadendsToView.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel([FromQuery] GetLineupPreviewChannelRequest request)
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLineupPreviewChannel.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLineupPreviewChannel.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetSelectedStationIdsRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSelectedStationIds.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSelectedStationIds.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StationChannelName>>> GetStationChannelNames()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetStationChannelNamesRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStationChannelNames.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStationChannelNames.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StationPreview>>> GetStationPreviews()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetStationPreviewsRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStationPreviews.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStationPreviews.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<HeadendDto>>> GetSubScribedHeadends()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetSubScribedHeadendsRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSubScribedHeadends.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSubScribedHeadends.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SubscribedLineup>>> GetSubscribedLineups()
        {
            try
            {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetSubscribedLineupsRequest())).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSubscribedLineups.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSubscribedLineups.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddHeadendToView(AddHeadendToViewRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddLineup(AddLineupRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddStation(AddStationRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveHeadendToView(RemoveHeadendToViewRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveLineup(RemoveLineupRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveStation(RemoveStationRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetStations(SetStationsRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISchedulesDirectHub
    {
        public async Task<List<CountryData>> GetAvailableCountries()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetAvailableCountriesRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<HeadendDto>> GetHeadendsByCountryPostal(GetHeadendsByCountryPostalRequest request)
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<HeadendToView>> GetHeadendsToView()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetHeadendsToViewRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(GetLineupPreviewChannelRequest request)
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<StationIdLineup>> GetSelectedStationIds()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetSelectedStationIdsRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<StationChannelName>> GetStationChannelNames()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetStationChannelNamesRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<StationPreview>> GetStationPreviews()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetStationPreviewsRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<HeadendDto>> GetSubScribedHeadends()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetSubScribedHeadendsRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<List<SubscribedLineup>> GetSubscribedLineups()
        {
             var ret = await APIStatsLogger.DebugAPI(Sender.Send(new GetSubscribedLineupsRequest())).ConfigureAwait(false);
            return ret.Data?? [];
        }
        public async Task<APIResponse?> AddHeadendToView(AddHeadendToViewRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> AddLineup(AddLineupRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> AddStation(AddStationRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> RemoveHeadendToView(RemoveHeadendToViewRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> RemoveLineup(RemoveLineupRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> RemoveStation(RemoveStationRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> SetStations(SetStationsRequest request)
        {
            var ret = await APIStatsLogger.DebugAPI(Sender.Send(request)).ConfigureAwait(false);
            return ret;
        }
    }
}
