using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannels.Commands;
using StreamMaster.Application.SMChannels.Queries;

namespace StreamMaster.Application.SMChannels.Controllers
{
    public partial class SMChannelsController(ILogger<SMChannelsController> _logger) : ApiControllerBase, ISMChannelsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<SMChannelDto>>> GetPagedSMChannels([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<SMChannelDto> ret = await Sender.Send(new GetPagedSMChannelsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> GetSMChannelNames()
        {
            try
            {
            DataResponse<List<string>> ret = await Sender.Send(new GetSMChannelNamesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSMChannelNames.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSMChannelNames.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<VideoInfo>> GetVideoInfoFromId([FromQuery] GetVideoInfoFromIdRequest request)
        {
            try
            {
            DataResponse<VideoInfo> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoInfoFromId.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoInfoFromId.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AutoSetEPG(AutoSetEPGRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AutoSetSMChannelNumbersFromParameters(AutoSetSMChannelNumbersFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AutoSetSMChannelNumbers(AutoSetSMChannelNumbersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CopySMChannel(CopySMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CreateSMChannel(CreateSMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CreateSMChannelsFromStreamParameters(CreateSMChannelsFromStreamParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CreateSMChannelsFromStreams(CreateSMChannelsFromStreamsRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelEPGId(SetSMChannelEPGIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelGroup(SetSMChannelGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelName(SetSMChannelNameRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelNumber(SetSMChannelNumberRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelsGroupFromParameters(SetSMChannelsGroupFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelsGroup(SetSMChannelsGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelsCommandProfileNameFromParameters(SetSMChannelsCommandProfileNameFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelsCommandProfileName(SetSMChannelsCommandProfileNameRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> SetSMChannelCommandProfileName(SetSMChannelCommandProfileNameRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> ToggleSMChannelsVisibleById(ToggleSMChannelsVisibleByIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> ToggleSMChannelVisibleById(ToggleSMChannelVisibleByIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> ToggleSMChannelVisibleByParameters(ToggleSMChannelVisibleByParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateSMChannel(UpdateSMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMChannelsHub
    {
        public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters Parameters)
        {
            PagedResponse<SMChannelDto> ret = await Sender.Send(new GetPagedSMChannelsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<List<string>> GetSMChannelNames()
        {
             DataResponse<List<string>> ret = await Sender.Send(new GetSMChannelNamesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<VideoInfo> GetVideoInfoFromId(GetVideoInfoFromIdRequest request)
        {
             DataResponse<VideoInfo> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AutoSetEPG(AutoSetEPGRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AutoSetSMChannelNumbersFromParameters(AutoSetSMChannelNumbersFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AutoSetSMChannelNumbers(AutoSetSMChannelNumbersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CopySMChannel(CopySMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CreateSMChannel(CreateSMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CreateSMChannelsFromStreamParameters(CreateSMChannelsFromStreamParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CreateSMChannelsFromStreams(CreateSMChannelsFromStreamsRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelEPGId(SetSMChannelEPGIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelGroup(SetSMChannelGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelName(SetSMChannelNameRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelNumber(SetSMChannelNumberRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelsGroupFromParameters(SetSMChannelsGroupFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelsGroup(SetSMChannelsGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelsCommandProfileNameFromParameters(SetSMChannelsCommandProfileNameFromParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelsCommandProfileName(SetSMChannelsCommandProfileNameRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> SetSMChannelCommandProfileName(SetSMChannelCommandProfileNameRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> ToggleSMChannelsVisibleById(ToggleSMChannelsVisibleByIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> ToggleSMChannelVisibleById(ToggleSMChannelVisibleByIdRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> ToggleSMChannelVisibleByParameters(ToggleSMChannelVisibleByParametersRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateSMChannel(UpdateSMChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
