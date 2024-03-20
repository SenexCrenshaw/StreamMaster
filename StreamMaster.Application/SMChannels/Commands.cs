using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannels;

namespace StreamMaster.Application.SMChannels
{
    public partial class SMChannelsController(ISMChannelsService SMChannelsService) : ApiControllerBase, ISMChannelsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<SMChannelDto>>> GetPagedSMChannels([FromQuery] SMChannelParameters Parameters)
        {
            APIResponse<SMChannelDto> ret = await SMChannelsService.GetPagedSMChannels(Parameters).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> CreateSMChannelFromStream(string streamId)
        {
            DefaultAPIResponse ret = await SMChannelsService.CreateSMChannelFromStream(streamId).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannels(List<int> smchannelIds)
        {
            DefaultAPIResponse ret = await SMChannelsService.DeleteSMChannels(smchannelIds).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannel(int smchannelId)
        {
            DefaultAPIResponse ret = await SMChannelsService.DeleteSMChannel(smchannelId).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteAllSMChannelsFromParameters(SMChannelParameters Parameters)
        {
            DefaultAPIResponse ret = await SMChannelsService.DeleteAllSMChannelsFromParameters(Parameters).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> AddSMStreamToSMChannel(SMStreamSMChannelRequest request)
        {
            DefaultAPIResponse ret = await SMChannelsService.AddSMStreamToSMChannel(request).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> RemoveSMStreamFromSMChannel(SMStreamSMChannelRequest request)
        {
            DefaultAPIResponse ret = await SMChannelsService.RemoveSMStreamFromSMChannel(request).ConfigureAwait(false);
            return ret.IsError.HasValue && ret.IsError.Value ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMChannelsHub
    {
        public async Task<APIResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters Parameters)
        {
            APIResponse<SMChannelDto> ret = await SMChannelsService.GetPagedSMChannels(Parameters).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> CreateSMChannelFromStream(string streamId)
        {
            DefaultAPIResponse ret = await SMChannelsService.CreateSMChannelFromStream(streamId).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteSMChannels(List<int> smchannelIds)
        {
            DefaultAPIResponse ret = await SMChannelsService.DeleteSMChannels(smchannelIds).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteSMChannel(int smchannelId)
        {
            DefaultAPIResponse ret = await SMChannelsService.DeleteSMChannel(smchannelId).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteAllSMChannelsFromParameters(SMChannelParameters Parameters)
        {
            DefaultAPIResponse ret = await SMChannelsService.DeleteAllSMChannelsFromParameters(Parameters).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> AddSMStreamToSMChannel(SMStreamSMChannelRequest request)
        {
            DefaultAPIResponse ret = await SMChannelsService.AddSMStreamToSMChannel(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(SMStreamSMChannelRequest request)
        {
            DefaultAPIResponse ret = await SMChannelsService.RemoveSMStreamFromSMChannel(request).ConfigureAwait(false);
            return ret;
        }

    }
}
