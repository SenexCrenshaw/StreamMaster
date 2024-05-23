using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Profiles.Commands;

namespace StreamMaster.Application.Profiles.Controllers
{
    public partial class ProfilesController(ILogger<ProfilesController> _logger) : ApiControllerBase, IProfilesController
    {        

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddFFMPEGProfile(AddFFMPEGProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IProfilesHub
    {
        public async Task<APIResponse> AddFFMPEGProfile(AddFFMPEGProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveFFMPEGProfile(RemoveFFMPEGProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateFFMPEGProfile(UpdateFFMPEGProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
