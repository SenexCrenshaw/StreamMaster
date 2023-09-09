using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups;

public interface IChannelGroupController
{

    Task<ActionResult> DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
    Task<ActionResult<IEnumerable<string>>> GetChannelGroupNames();
    Task<ActionResult<IEnumerable<ChannelGroupIdName>>> GetChannelGroupIdNames();


    Task<ActionResult> CreateChannelGroup(CreateChannelGroupRequest request);

    Task<ActionResult> DeleteChannelGroup(DeleteChannelGroupRequest request);

    Task<ActionResult<ChannelGroupDto>> GetChannelGroup(int id);

    Task<ActionResult<PagedResponse<ChannelGroupDto>>> GetChannelGroups(ChannelGroupParameters Parameters);

    Task<ActionResult> UpdateChannelGroup(UpdateChannelGroupRequest request);

    Task<ActionResult> UpdateChannelGroups(UpdateChannelGroupsRequest request);

}

public interface IChannelGroupDB
{
}

public interface IChannelGroupHub
{

    Task DeleteAllChannelGroupsFromParameters(DeleteAllChannelGroupsFromParametersRequest request);
    Task<IEnumerable<string>> GetChannelGroupNames();
    Task<IEnumerable<ChannelGroupIdName>> GetChannelGroupIdNames();
    Task CreateChannelGroup(CreateChannelGroupRequest request);

    Task DeleteChannelGroup(DeleteChannelGroupRequest request);

    Task<ChannelGroupDto?> GetChannelGroup(int id);

    Task<PagedResponse<ChannelGroupDto>> GetChannelGroups(ChannelGroupParameters channelGroupParameters);


    Task UpdateChannelGroup(UpdateChannelGroupRequest request);

    Task UpdateChannelGroups(UpdateChannelGroupsRequest request);


}

public interface IChannelGroupScoped
{
}

public interface IChannelGroupTasks
{
    ValueTask UpdateChannelGroupCounts(CancellationToken cancellationToken = default);
}