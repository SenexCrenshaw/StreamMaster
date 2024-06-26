using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.StreamGroups.Commands;
using StreamMaster.Application.StreamGroups.Queries;

namespace StreamMaster.Application.StreamGroups
{
    public interface IStreamGroupsController
    {        
        Task<ActionResult<PagedResponse<StreamGroupDto>>> GetPagedStreamGroups(QueryStringParameters Parameters);
        Task<ActionResult<List<StreamGroupProfile>>> GetStreamGroupProfiles();
        Task<ActionResult<StreamGroupDto>> GetStreamGroup(GetStreamGroupRequest request);
        Task<ActionResult<List<StreamGroupDto>>> GetStreamGroups();
        Task<ActionResult<APIResponse>> AddProfileToStreamGroup(AddProfileToStreamGroupRequest request);
        Task<ActionResult<APIResponse>> CreateStreamGroup(CreateStreamGroupRequest request);
        Task<ActionResult<APIResponse>> DeleteStreamGroup(DeleteStreamGroupRequest request);
        Task<ActionResult<APIResponse>> RemoveStreamGroupProfile(RemoveStreamGroupProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateStreamGroupProfile(UpdateStreamGroupProfileRequest request);
        Task<ActionResult<APIResponse>> UpdateStreamGroup(UpdateStreamGroupRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IStreamGroupsHub
    {
        Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters);
        Task<List<StreamGroupProfile>> GetStreamGroupProfiles();
        Task<StreamGroupDto> GetStreamGroup(GetStreamGroupRequest request);
        Task<List<StreamGroupDto>> GetStreamGroups();
        Task<APIResponse> AddProfileToStreamGroup(AddProfileToStreamGroupRequest request);
        Task<APIResponse> CreateStreamGroup(CreateStreamGroupRequest request);
        Task<APIResponse> DeleteStreamGroup(DeleteStreamGroupRequest request);
        Task<APIResponse> RemoveStreamGroupProfile(RemoveStreamGroupProfileRequest request);
        Task<APIResponse> UpdateStreamGroupProfile(UpdateStreamGroupProfileRequest request);
        Task<APIResponse> UpdateStreamGroup(UpdateStreamGroupRequest request);
    }
}
