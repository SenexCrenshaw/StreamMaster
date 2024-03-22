using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannels.Commands;

namespace StreamMaster.Application.SMChannels
{
    public partial class SMChannelsController(ISender Sender) : ApiControllerBase
    {        

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> AddSMStreamToSMChannel(AddSMStreamToSMChannel request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> CreateSMChannelFromStream(CreateSMChannelFromStream request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteAllSMChannelsFromParameters(DeleteAllSMChannelsFromParameters request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannel(DeleteSMChannel request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> DeleteSMChannels(DeleteSMChannels request)
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
        public async Task<ActionResult<DefaultAPIResponse>> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannel request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SetSMChannelLogo(SetSMChannelLogo request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<ActionResult<DefaultAPIResponse>> SetSMStreamRanks(SetSMStreamRanks request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound() : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub 
    {
        public async Task<DefaultAPIResponse?> AddSMStreamToSMChannel(AddSMStreamToSMChannel request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> CreateSMChannelFromStream(CreateSMChannelFromStream request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> DeleteAllSMChannelsFromParameters(DeleteAllSMChannelsFromParameters request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> DeleteSMChannel(DeleteSMChannel request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> DeleteSMChannels(DeleteSMChannels request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse<SMChannelDto>?> GetPagedSMChannels(GetPagedSMChannels request)
        {
            APIResponse<SMChannelDto>? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> RemoveSMStreamFromSMChannel(RemoveSMStreamFromSMChannel request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> SetSMChannelLogo(SetSMChannelLogo request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<DefaultAPIResponse?> SetSMStreamRanks(SetSMStreamRanks request)
        {
            DefaultAPIResponse? ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
