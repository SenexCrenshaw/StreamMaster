using Microsoft.AspNetCore.Mvc;
using StreamMaster.Application.ChannelGroups.Commands;
using StreamMaster.Application.ChannelGroups.Queries;

namespace StreamMaster.Application.ChannelGroups
{
    public interface IChannelGroupsController
    {        
        Task<ActionResult<PagedResponse<ChannelGroupDto>>> GetPagedChannelGroups(QueryStringParameters Parameters);
        Task<ActionResult<DefaultAPIResponse>> CreateChannelGroup(CreateChannelGroupRequest request);
        Task<ActionResult<DefaultAPIResponse>> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
        Task<ActionResult<DefaultAPIResponse>> DeleteChannelGroup(DeleteChannelGroupRequest request);
        Task<ActionResult<DefaultAPIResponse>> UpdateChannelGroup(UpdateChannelGroupRequest request);
    }
}

namespace StreamMaster.Application.Hubs
{
    public interface IChannelGroupsHub
    {
        Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(QueryStringParameters Parameters);
        Task<DefaultAPIResponse> CreateChannelGroup(CreateChannelGroupRequest request);
        Task<DefaultAPIResponse> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
        Task<DefaultAPIResponse> DeleteChannelGroup(DeleteChannelGroupRequest request);
        Task<DefaultAPIResponse> UpdateChannelGroup(UpdateChannelGroupRequest request);
    }
}
