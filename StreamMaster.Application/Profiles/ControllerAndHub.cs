using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.Profiles.Commands;
using StreamMaster.Application.Profiles.Queries;

namespace StreamMaster.Application.Profiles.Controllers
{
    public partial class ProfilesController(ILogger<ProfilesController> _logger) : ApiControllerBase, IProfilesController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<FileOutputProfileDto>>> GetFileProfiles()
        {
            try
            {
            DataResponse<List<FileOutputProfileDto>> ret = await Sender.Send(new GetFileProfilesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetFileProfiles.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetFileProfiles.");
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
        public async Task<ActionResult<APIResponse>> AddFileProfile(AddFileProfileRequest request)
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
        public async Task<ActionResult<APIResponse>> RemoveFileProfile(RemoveFileProfileRequest request)
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
        public async Task<ActionResult<APIResponse>> UpdateFileProfile(UpdateFileProfileRequest request)
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
        public async Task<List<FileOutputProfileDto>> GetFileProfiles()
        {
             DataResponse<List<FileOutputProfileDto>> ret = await Sender.Send(new GetFileProfilesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<VideoOutputProfileDto>> GetVideoProfiles()
        {
             DataResponse<List<VideoOutputProfileDto>> ret = await Sender.Send(new GetVideoProfilesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> AddFileProfile(AddFileProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> AddVideoProfile(AddVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveFileProfile(RemoveFileProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveVideoProfile(RemoveVideoProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateFileProfile(UpdateFileProfileRequest request)
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
