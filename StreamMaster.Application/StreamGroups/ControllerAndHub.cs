using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups.Controllers
{
    [Authorize]
    public partial class StreamGroupsController(ILogger<StreamGroupsController> _logger) : ApiControllerBase, IStreamGroupsController
    {
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups([FromQuery] QueryStringParameters Parameters)
        {
            var ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret?? new();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StreamGroupProfile>>> GetStreamGroupProfiles()
        {
            try
            {
            var ret = await Sender.Send(new GetStreamGroupProfilesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroupProfiles.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamGroupProfiles.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<StreamGroupDto>> GetStreamGroup([FromQuery] GetStreamGroupRequest request)
        {
            try
            {
            var ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroup.", statusCode: 500) : Ok(ret.Data?? new());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamGroup.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StreamGroupDto>>> GetStreamGroups()
        {
            try
            {
            var ret = await Sender.Send(new GetStreamGroupsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroups.", statusCode: 500) : Ok(ret.Data?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamGroups.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> AddProfileToStreamGroup(AddProfileToStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> CreateStreamGroup(CreateStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> DeleteStreamGroup(DeleteStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> RemoveStreamGroupProfile(RemoveStreamGroupProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> UpdateStreamGroupProfile(UpdateStreamGroupProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse?>> UpdateStreamGroup(UpdateStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }
    }
}

namespace StreamMaster.Application.Hubs
{
    public partial class StreamMasterHub : IStreamGroupsHub
    {
        public async Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters)
        {
            var ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret?? new();
        }

        public async Task<List<StreamGroupProfile>> GetStreamGroupProfiles()
        {
             var ret = await Sender.Send(new GetStreamGroupProfilesRequest()).ConfigureAwait(false);
            return ret.Data?? [];
        }

        public async Task<StreamGroupDto> GetStreamGroup(GetStreamGroupRequest request)
        {
             var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data?? new();
        }

        public async Task<List<StreamGroupDto>> GetStreamGroups()
        {
             var ret = await Sender.Send(new GetStreamGroupsRequest()).ConfigureAwait(false);
            return ret.Data?? [];
        }

        public async Task<APIResponse?> AddProfileToStreamGroup(AddProfileToStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> CreateStreamGroup(CreateStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> DeleteStreamGroup(DeleteStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> RemoveStreamGroupProfile(RemoveStreamGroupProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> UpdateStreamGroupProfile(UpdateStreamGroupProfileRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse?> UpdateStreamGroup(UpdateStreamGroupRequest request)
        {
            var ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }
    }
}
