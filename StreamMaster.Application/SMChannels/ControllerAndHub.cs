using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannels.Commands;
using StreamMaster.Application.SMChannels.Queries;

namespace StreamMaster.Application.SMChannels
{
    public partial class SMChannelsController(ISender Sender) : ApiControllerBase, ISMChannelsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<SMChannelDto>>> GetPagedSMChannels([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<SMChannelDto> ret = await Sender.Send(new GetPagedSMChannelsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> CreateSMChannelFromStream(CreateSMChannelFromStreamRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SetSMChannelName(SetSMChannelNameRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SetSMChannelNumber(SetSMChannelNumberRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SetSMStreamRanks(SetSMStreamRanksRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
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

        public async Task<DefaultAPIResponse> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> CreateSMChannelFromStream(CreateSMChannelFromStreamRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SetSMChannelName(SetSMChannelNameRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SetSMChannelNumber(SetSMChannelNumberRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse> SetSMStreamRanks(SetSMStreamRanksRequest request)
        {
            DefaultAPIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
