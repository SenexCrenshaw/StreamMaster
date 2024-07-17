using Microsoft.AspNetCore.Mvc;

using StreamMaster.Application.Profiles.Commands;
using StreamMaster.Application.Profiles.Queries;

namespace StreamMaster.Application.Profiles
{
    public partial class ProfilesController(ILogger<ProfilesController> _logger) : ApiControllerBase, IProfilesController
    {

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<OutputProfileDto>> GetOutputProfile([FromQuery] GetOutputProfileRequest request)
        {
            try
            {
                DataResponse<OutputProfileDto> ret = await Sender.Send(request).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetOutputProfile.", statusCode: 500) : Ok(ret.Data);
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
                DataResponse<List<OutputProfileDto>> ret = await Sender.Send(new GetOutputProfilesRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetOutputProfiles.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetOutputProfiles.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<VideoOutputProfileDto>>> GetVideoProfiles()
        {
            try
            {
                DataResponse<List<VideoOutputProfileDto>> ret = await Sender.Send(new GetVideoProfilesRequest()).ConfigureAwait(false);
                return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetVideoProfiles.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetVideoProfiles.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddOutputProfile(AddOutputProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddVideoProfile(AddVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveOutputProfile(RemoveOutputProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveVideoProfile(RemoveVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateOutputProfile(UpdateOutputProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateVideoProfile(UpdateVideoProfileRequest request)
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
        public async Task<OutputProfileDto> GetOutputProfile(GetOutputProfileRequest request)
        {
            DataResponse<OutputProfileDto> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<OutputProfileDto>> GetOutputProfiles()
        {
            DataResponse<List<OutputProfileDto>> ret = await Sender.Send(new GetOutputProfilesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<VideoOutputProfileDto>> GetVideoProfiles()
        {
            DataResponse<List<VideoOutputProfileDto>> ret = await Sender.Send(new GetVideoProfilesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> AddOutputProfile(AddOutputProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AddVideoProfile(AddVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveOutputProfile(RemoveOutputProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveVideoProfile(RemoveVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateOutputProfile(UpdateOutputProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateVideoProfile(UpdateVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
