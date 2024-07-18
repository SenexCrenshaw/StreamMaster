using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Streaming.Commands;
using StreamMaster.Application.Streaming.Queries;

namespace StreamMaster.Application.Streaming.Controllers
{
    public partial class StreamingController(ILogger<StreamingController> _logger) : ApiControllerBase, IStreamingController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<IdNameUrl>>> GetVideoStreamNamesAndUrls()
        {
            try
            {
            DataResponse<List<IdNameUrl>> ret = await Sender.Send(new GetVideoStreamNamesAndUrlsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoStreamNamesAndUrls.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoStreamNamesAndUrls.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CancelAllChannels()
        {
            APIResponse ret = await Sender.Send(new CancelAllChannelsRequest()).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CancelChannel(CancelChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CancelClient(CancelClientRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> MoveToNextStream(MoveToNextStreamRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStreamingHub
    {
        public async Task<List<IdNameUrl>> GetVideoStreamNamesAndUrls()
        {
             DataResponse<List<IdNameUrl>> ret = await Sender.Send(new GetVideoStreamNamesAndUrlsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> CancelAllChannels()
        {
            APIResponse ret = await Sender.Send(new CancelAllChannelsRequest()).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CancelChannel(CancelChannelRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CancelClient(CancelClientRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> MoveToNextStream(MoveToNextStreamRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
