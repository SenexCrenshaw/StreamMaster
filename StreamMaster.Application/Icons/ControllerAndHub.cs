using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Icons.Queries;

namespace StreamMaster.Application.Icons
{
    public partial class IconsController(ISender Sender) : ApiControllerBase, IIconsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<List<IconFileDto>> GetIcons()
        {
            List<IconFileDto> ret = await Sender.Send(new GetIconsRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IIconsHub
    {
        public async Task<List<IconFileDto>> GetIcons()
        {
            List<IconFileDto> ret = await Sender.Send(new GetIconsRequest()).ConfigureAwait(false);
            return ret;
        }

    }
}
