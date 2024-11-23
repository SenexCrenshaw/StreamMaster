using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.SMChannels.Commands;
using StreamMaster.Application.SMChannels.Queries;

namespace StreamMaster.Application.SMChannels.Controllers
{
    [Authorize]
    public partial class SMChannelsController(ILogger<SMChannelsController> _logger) : ApiControllerBase, ISMChannelsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<SMChannelDto>>> GetPagedSMChannels([FromQuery] QueryStringParameters Parameters)
        {
            var ret = await Sender.Send(new GetPagedSMChannelsRequest(Parameters)).ConfigureAwait(false);
            return ret?? new();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<string>>> GetSMChannelNames()
        {
            try
            {
            var ret = await Sender.Send(new GetSMChannelNamesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSMChannelNames.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSMChannelNames.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<IdNameUrl>>> GetVideoStreamNamesAndUrls()
        {
            try
            {
            var ret = await Sender.Send(new GetVideoStreamNamesAndUrlsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoStreamNamesAndUrls.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoStreamNamesAndUrls.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AutoSetEPG(AutoSetEPGRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AutoSetSMChannelNumbersFromParameters(AutoSetSMChannelNumbersFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AutoSetSMChannelNumbers(AutoSetSMChannelNumbersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CopySMChannel(CopySMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CreateMultiViewChannel(CreateMultiViewChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CreateSMChannel(CreateSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CreateSMChannelsFromStreamParameters(CreateSMChannelsFromStreamParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CreateSMChannelsFromStreams(CreateSMChannelsFromStreamsRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelEPGId(SetSMChannelEPGIdRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelGroup(SetSMChannelGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelName(SetSMChannelNameRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelNumber(SetSMChannelNumberRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelsGroupFromParameters(SetSMChannelsGroupFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelsGroup(SetSMChannelsGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelsCommandProfileNameFromParameters(SetSMChannelsCommandProfileNameFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelsCommandProfileName(SetSMChannelsCommandProfileNameRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> SetSMChannelCommandProfileName(SetSMChannelCommandProfileNameRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> ToggleSMChannelsVisibleById(ToggleSMChannelsVisibleByIdRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> ToggleSMChannelVisibleById(ToggleSMChannelVisibleByIdRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> ToggleSMChannelVisibleByParameters(ToggleSMChannelVisibleByParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> UpdateMultiViewChannel(UpdateMultiViewChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> UpdateSMChannel(UpdateSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
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
            var ret = await Sender.Send(new GetPagedSMChannelsRequest(Parameters)).ConfigureAwait(false);
            return ret?? new();
        }

        public async Task<List<string>> GetSMChannelNames()
        {
             var ret = await Sender.Send(new GetSMChannelNamesRequest()).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<List<IdNameUrl>> GetVideoStreamNamesAndUrls()
        {
             var ret = await Sender.Send(new GetVideoStreamNamesAndUrlsRequest()).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<APIResponse?> AutoSetEPGFromParameters(AutoSetEPGFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> AutoSetEPG(AutoSetEPGRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> AutoSetSMChannelNumbersFromParameters(AutoSetSMChannelNumbersFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> AutoSetSMChannelNumbers(AutoSetSMChannelNumbersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> CopySMChannel(CopySMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> CreateMultiViewChannel(CreateMultiViewChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> CreateSMChannel(CreateSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> CreateSMChannelsFromStreamParameters(CreateSMChannelsFromStreamParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> CreateSMChannelsFromStreams(CreateSMChannelsFromStreamsRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelEPGId(SetSMChannelEPGIdRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelGroup(SetSMChannelGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelName(SetSMChannelNameRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelNumber(SetSMChannelNumberRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelsGroupFromParameters(SetSMChannelsGroupFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelsGroup(SetSMChannelsGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelsCommandProfileNameFromParameters(SetSMChannelsCommandProfileNameFromParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelsCommandProfileName(SetSMChannelsCommandProfileNameRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> SetSMChannelCommandProfileName(SetSMChannelCommandProfileNameRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> ToggleSMChannelsVisibleById(ToggleSMChannelsVisibleByIdRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> ToggleSMChannelVisibleById(ToggleSMChannelVisibleByIdRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> ToggleSMChannelVisibleByParameters(ToggleSMChannelVisibleByParametersRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> UpdateMultiViewChannel(UpdateMultiViewChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> UpdateSMChannel(UpdateSMChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
