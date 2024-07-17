using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.EPG.Queries;

namespace StreamMaster.Application.EPG
{
    public partial class EPGController(ILogger<EPGController> _logger) : ApiControllerBase, IEPGController
    {

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<EPGColorDto>>> GetEPGColors()
        {
            try
            {
                DataResponse<List<EPGColorDto>> ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetEPGColors.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetEPGColors.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IEPGHub
    {
        public async Task<List<EPGColorDto>> GetEPGColors()
        {
            DataResponse<List<EPGColorDto>> ret = await Sender.Send(new GetEPGColorsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
