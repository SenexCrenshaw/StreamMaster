using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Streaming.Commands;

namespace StreamMaster.Application.Streaming.Controllers
{
    [Authorize]
    public partial class StreamingController() : ApiControllerBase, IStreamingController
    {
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CancelAllChannels()
        {
            var ret = await Sender.Send(new CancelAllChannelsRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CancelChannel(CancelChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CancelClient(CancelClientRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> MoveToNextStream(MoveToNextStreamRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStreamingHub
    {
        public async Task<APIResponse?> CancelAllChannels()
        {
            var ret = await Sender.Send(new CancelAllChannelsRequest()).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> CancelChannel(CancelChannelRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> CancelClient(CancelClientRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
        public async Task<APIResponse?> MoveToNextStream(MoveToNextStreamRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
