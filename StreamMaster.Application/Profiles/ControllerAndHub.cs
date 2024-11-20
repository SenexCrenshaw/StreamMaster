using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.Profiles.Commands;
using StreamMaster.Application.Profiles.Queries;

namespace StreamMaster.Application.Profiles.Controllers
{
    [Authorize]
    public partial class ProfilesController(ILogger<ProfilesController> _logger) : ApiControllerBase, IProfilesController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<CommandProfileDto>>> GetCommandProfiles()
        {
            try
            {
            var ret = await Sender.Send(new GetCommandProfilesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetCommandProfiles.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetCommandProfiles.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<OutputProfileDto>> GetOutputProfile([FromQuery] GetOutputProfileRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetOutputProfile.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetOutputProfile.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<OutputProfileDto>>> GetOutputProfiles()
        {
            try
            {
            var ret = await Sender.Send(new GetOutputProfilesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetOutputProfiles.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetOutputProfiles.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddCommandProfile(AddCommandProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddOutputProfile(AddOutputProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveCommandProfile(RemoveCommandProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveOutputProfile(RemoveOutputProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> UpdateCommandProfile(UpdateCommandProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> UpdateOutputProfile(UpdateOutputProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IProfilesHub
    {
        public async Task<List<CommandProfileDto>> GetCommandProfiles()
        {
             var ret = await Sender.Send(new GetCommandProfilesRequest()).ConfigureAwait(false);
            return ret.Data?? [];
        }

        public async Task<OutputProfileDto> GetOutputProfile(GetOutputProfileRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<List<OutputProfileDto>> GetOutputProfiles()
        {
             var ret = await Sender.Send(new GetOutputProfilesRequest()).ConfigureAwait(false);
            return ret.Data?? [];
        }

        public async Task<APIResponse?> AddCommandProfile(AddCommandProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> AddOutputProfile(AddOutputProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> RemoveCommandProfile(RemoveCommandProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> RemoveOutputProfile(RemoveOutputProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> UpdateCommandProfile(UpdateCommandProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> UpdateOutputProfile(UpdateOutputProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
