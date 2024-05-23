using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups
{
    public interface IStreamGroupsController
    {        
        Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups(QueryStringParameters Parameters);
        Task<ActionResult<StreamGroupDto>> GetStreamGroup(GetStreamGroupRequest request);
        Task<ActionResult<List<StreamGroupDto>>> GetStreamGroups();
        Task<ActionResult<APIResponse>> CreateStreamGroup(CreateStreamGroupRequest request);
        Task<ActionResult<APIResponse>> DeleteStreamGroup(DeleteStreamGroupRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStreamGroupsHub
    {
        Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters);
        Task<StreamGroupDto> GetStreamGroup(GetStreamGroupRequest request);
        Task<List<StreamGroupDto>> GetStreamGroups();
        Task<APIResponse> CreateStreamGroup(CreateStreamGroupRequest request);
        Task<APIResponse> DeleteStreamGroup(DeleteStreamGroupRequest request);
    }
}
