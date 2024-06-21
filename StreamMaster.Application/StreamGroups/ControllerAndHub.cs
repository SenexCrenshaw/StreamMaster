using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups.Controllers
{
    public partial class StreamGroupsController(ILogger<StreamGroupsController> _logger) : ApiControllerBase, IStreamGroupsController
    {        

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups([FromQuery] QueryStringParameters Parameters)
        {
            PagedResponse<StreamGroupDto> ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<StreamGroupProfile>>> GetStreamGroupProfiles()
        {
            try
            {
            DataResponse<List<StreamGroupProfile>> ret = await Sender.Send(new GetStreamGroupProfilesRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroupProfiles.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamGroupProfiles.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<StreamGroupDto>> GetStreamGroup(GetStreamGroupRequest request)
        {
            try
            {
            DataResponse<StreamGroupDto> ret = await Sender.Send(request).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroup.", statusCode: 500) : Ok(ret.Data);
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
            DataResponse<List<StreamGroupDto>> ret = await Sender.Send(new GetStreamGroupsRequest()).ConfigureAwait(false);
             return ret.IsError ? Problem(detail: "An unexpected error occurred retrieving GetStreamGroups.", statusCode: 500) : Ok(ret.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while processing the request to get GetStreamGroups.");
                return Problem(detail: "An unexpected error occurred. Please try again later.", statusCode: 500);
            }
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> AddProfileToStreamGroup(AddProfileToStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> CreateStreamGroup(CreateStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> DeleteStreamGroup(DeleteStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpDelete]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> RemoveStreamGroupProfile(RemoveStreamGroupProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateStreamGroupProfile(UpdateStreamGroupProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret == null ? NotFound(ret) : Ok(ret);
        }

        [HttpPatch]
        [Route("[action]")]
        public async Task<ActionResult<APIResponse>> UpdateStreamGroup(UpdateStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
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
            PagedResponse<StreamGroupDto> ret = await Sender.Send(new GetPagedStreamGroupsRequest(Parameters)).ConfigureAwait(false);
            return ret;
        }

        public async Task<List<StreamGroupProfile>> GetStreamGroupProfiles()
        {
             DataResponse<List<StreamGroupProfile>> ret = await Sender.Send(new GetStreamGroupProfilesRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<StreamGroupDto> GetStreamGroup(GetStreamGroupRequest request)
        {
             DataResponse<StreamGroupDto> ret = await Sender.Send(request).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<List<StreamGroupDto>> GetStreamGroups()
        {
             DataResponse<List<StreamGroupDto>> ret = await Sender.Send(new GetStreamGroupsRequest()).ConfigureAwait(false);
            return ret.Data;
        }

        public async Task<APIResponse> AddProfileToStreamGroup(AddProfileToStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> CreateStreamGroup(CreateStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> DeleteStreamGroup(DeleteStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> RemoveStreamGroupProfile(RemoveStreamGroupProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateStreamGroupProfile(UpdateStreamGroupProfileRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

        public async Task<APIResponse> UpdateStreamGroup(UpdateStreamGroupRequest request)
        {
            APIResponse ret = await Sender.Send(request).ConfigureAwait(false);
            return ret;
        }

    }
}
