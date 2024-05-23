namespace StreamMaster.Application.VideoStreams
{
    public partial class VideoStreamsController(ILogger<VideoStreamsController> _logger) : ApiControllerBase
    {

        //[HttpPatch]
        //[Route("[action]")]
        //public async Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request)
        //{
        //    APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
        //    return ret == null ? NotFound(ret) : Ok(ret);
        //}

        //[HttpPatch]
        //[Route("[action]")]
        //public async Task<ActionResult<APIResponse>> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request)
        //{
        //    APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
        //    return ret == null ? NotFound(ret) : Ok(ret);
        //}

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IVideoStreamsHub
    {
        //public async Task<APIResponse> SetSMChannelsLogoFromEPGFromParameters(SetSMChannelsLogoFromEPGFromParametersRequest request)
        //{
        //    APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
        //    return ret;
        //}

        //public async Task<APIResponse> SetSMChannelsLogoFromEPG(SetSMChannelsLogoFromEPGRequest request)
        //{
        //    APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
        //    return ret;
        //}

    }
}
