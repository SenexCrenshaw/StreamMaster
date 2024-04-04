using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups
{
    public interface IChannelGroupsController
    {        
    Task<ActionResult<PagedResponse<ChannelGroupDto>>> GetPagedChannelGroups(QueryStringParameters Parameters);
    Task<ActionResult<DefaultAPIResponse>> CreateChannelGroup(CreateChannelGroupRequest request);
    Task<ActionResult<bool>> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
    Task<ActionResult<bool>> DeleteChannelGroup(DeleteChannelGroupRequest request);
    Task<ActionResult<ChannelGroupDto?>> UpdateChannelGroup(UpdateChannelGroupRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IChannelGroupsHub
    {
        Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(QueryStringParameters Parameters);
        Task<DefaultAPIResponse> CreateChannelGroup(CreateChannelGroupRequest request);
        Task<bool> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
        Task<bool> DeleteChannelGroup(DeleteChannelGroupRequest request);
        Task<ChannelGroupDto?> UpdateChannelGroup(UpdateChannelGroupRequest request);
    }
}
