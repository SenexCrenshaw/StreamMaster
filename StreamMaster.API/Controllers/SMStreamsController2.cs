namespace StreamMaster.API.Controllers;

//public class SMStreamsController : ApiControllerBase, ISMStreamsController
//{
//    [HttpGet]
//    [Route("[action]")]
//    public async Task<ActionResult<PagedResponse<SMStreamDto>>> GetPagedSMStreams([FromQuery] SMStreamParameters Parameters)
//    {
//        PagedResponse<SMStreamDto> ret = await Mediator.Send(new GetPagedSMStreams(Parameters)).ConfigureAwait(false);
//        return ret;
//    }

//    //[HttpGet]
//    //[Route("[action]")]
//    //public async Task<ActionResult<bool>> ToggleSMStreamVisible(ToggleSMStreamVisibleRequest request)
//    //{
//    //    bool ret = await Mediator.Send(request).ConfigureAwait(false);
//    //    return ret;
//    //}
//}
