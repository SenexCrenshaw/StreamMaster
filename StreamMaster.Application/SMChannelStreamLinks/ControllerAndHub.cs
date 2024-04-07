using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.SMChannelStreamLinks.Queries;

namespace StreamMaster.Application.SMChannelStreamLinks.Controllers
{
    public partial class SMChannelStreamLinksController(ISender Sender, ILogger<SMChannelStreamLinksController> _logger) : ApiControllerBase, ISMChannelStreamLinksController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<SMStreamDto>>> GetSMChannelStreams(int SMChannelId)
        {
            try
            {
            DataResponse<List<SMStreamDto>> ret = await Sender.Send(new GetSMChannelStreamsRequest(SMChannelId)).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetSMChannelStreams.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetSMChannelStreams.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : ISMChannelStreamLinksHub
    {
        public async Task<List<SMStreamDto>> GetSMChannelStreams(int SMChannelId)
        {
             DataResponse<List<SMStreamDto>> ret = await Sender.Send(new GetSMChannelStreamsRequest(SMChannelId)).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
