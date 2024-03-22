using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannels.Commands;

namespace StreamMaster.Application.SMChannels
{
    public partial class SMChannelsController(ISender Sender) : ApiControllerBase, ISMChannelsController
    {        

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> CreateSMChannelFromStream(CreateSMChannelFromStreamRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse<SMChannelDto>>> GetPagedSMChannels([FromQuery] SMChannelParameters Parameters)
        {
            APIResponse<SMChannelDto> ret = await Sender.Send(new GetPagedSMChannels(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse?>> SetSMStreamRanks(SetSMStreamRanksRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMChannelsHub
    {
        public async Task<DefaultAPIResponse?> AddSMStreamToSMChannel(AddSMStreamToSMChannelRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> CreateSMChannelFromStream(CreateSMChannelFromStreamRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> DeleteSMChannel(DeleteSMChannelRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> DeleteSMChannelsFromParameters(DeleteSMChannelsFromParametersRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> DeleteSMChannels(DeleteSMChannelsRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse<SMChannelDto>> GetPagedSMChannels(SMChannelParameters Parameters)
        {
            APIResponse<SMChannelDto> ret = await Sender.Send(new GetPagedSMChannels(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannelRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> SetSMChannelLogo(SetSMChannelLogoRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> SetSMStreamRanks(SetSMStreamRanksRequest request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
