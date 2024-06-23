using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SchedulesDirect.Commands;
using StreamMaster.Application.SchedulesDirect.Queries;

namespace StreamMaster.Application.SchedulesDirect.Controllers
{
    public partial class SchedulesDirectController(ILogger<SchedulesDirectController> _logger) : ApiControllerBase, ISchedulesDirectController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CountryData>>> GetAvailableCountries()
        {
            try
            {
            DataResponse<List<CountryData>> ret = await Sender.Send(new GetAvailableCountriesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetAvailableCountries.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetAvailableCountries.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<HeadendDto>>> GetHeadends([FromQuery] GetHeadendsRequest request)
        {
            try
            {
            DataResponse<List<HeadendDto>> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetHeadends.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetHeadends.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<LineupPreviewChannel>>> GetLineupPreviewChannel([FromQuery] GetLineupPreviewChannelRequest request)
        {
            try
            {
            DataResponse<List<LineupPreviewChannel>> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLineupPreviewChannel.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLineupPreviewChannel.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SubscribedLineup>>> GetLineups()
        {
            try
            {
            DataResponse<List<SubscribedLineup>> ret = await Sender.Send(new GetLineupsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetLineups.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetLineups.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StationIdLineup>>> GetSelectedStationIds()
        {
            try
            {
            DataResponse<List<StationIdLineup>> ret = await Sender.Send(new GetSelectedStationIdsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSelectedStationIds.", statusCode: 500) : Ok(ret.Data);
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
            DataResponse<List<StationChannelName>> ret = await Sender.Send(new GetStationChannelNamesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStationChannelNames.", statusCode: 500) : Ok(ret.Data);
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
            DataResponse<List<StationPreview>> ret = await Sender.Send(new GetStationPreviewsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStationPreviews.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStationPreviews.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddLineup(AddLineupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddStation(AddStationRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveLineup(RemoveLineupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveStation(RemoveStationRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
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
             DataResponse<List<CountryData>> ret = await Sender.Send(new GetAvailableCountriesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<HeadendDto>> GetHeadends(GetHeadendsRequest request)
        {
             DataResponse<List<HeadendDto>> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<LineupPreviewChannel>> GetLineupPreviewChannel(GetLineupPreviewChannelRequest request)
        {
             DataResponse<List<LineupPreviewChannel>> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<SubscribedLineup>> GetLineups()
        {
             DataResponse<List<SubscribedLineup>> ret = await Sender.Send(new GetLineupsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<StationIdLineup>> GetSelectedStationIds()
        {
             DataResponse<List<StationIdLineup>> ret = await Sender.Send(new GetSelectedStationIdsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<StationChannelName>> GetStationChannelNames()
        {
             DataResponse<List<StationChannelName>> ret = await Sender.Send(new GetStationChannelNamesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<StationPreview>> GetStationPreviews()
        {
             DataResponse<List<StationPreview>> ret = await Sender.Send(new GetStationPreviewsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> AddLineup(AddLineupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AddStation(AddStationRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveLineup(RemoveLineupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveStation(RemoveStationRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
