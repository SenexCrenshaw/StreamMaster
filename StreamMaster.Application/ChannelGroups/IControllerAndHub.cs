using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups
{
    public interface IChannelGroupsController
    {        
        Task<ActionResult<List<ChannelGroupDto>>> GetChannelGroups();
        Task<ActionResult<PagedResponse<ChannelGroupDto>>> GetPagedChannelGroups(QueryStringParameters Parameters);
        Task<ActionResult<APIResponse>> CreateChannelGroup(CreateChannelGroupRequest request);
        Task<ActionResult<APIResponse>> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
        Task<ActionResult<APIResponse>> DeleteChannelGroup(DeleteChannelGroupRequest request);
        Task<ActionResult<APIResponse>> UpdateChannelGroup(UpdateChannelGroupRequest request);
        Task<ActionResult<APIResponse>> UpdateChannelGroups(UpdateChannelGroupsRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IChannelGroupsHub
    {
        Task<List<ChannelGroupDto>> GetChannelGroups();
        Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(QueryStringParameters Parameters);
        Task<APIResponse> CreateChannelGroup(CreateChannelGroupRequest request);
        Task<APIResponse> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
        Task<APIResponse> DeleteChannelGroup(DeleteChannelGroupRequest request);
        Task<APIResponse> UpdateChannelGroup(UpdateChannelGroupRequest request);
        Task<APIResponse> UpdateChannelGroups(UpdateChannelGroupsRequest request);
    }
}
