using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Icons.Queries;

namespace StreamMaster.Application.Icons
{
    public partial class IconsController(ILogger<IconsController> _logger) : ApiControllerBase, IIconsController
    {

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<LogoFileDto>>> GetIcons()
        {
            try
            {
                DataResponse<List<LogoFileDto>> ret = await Sender.Send(new GetIconsRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetIcons.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetIcons.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IIconsHub
    {
        public async Task<List<LogoFileDto>> GetIcons()
        {
            DataResponse<List<LogoFileDto>> ret = await Sender.Send(new GetIconsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

    }
}
