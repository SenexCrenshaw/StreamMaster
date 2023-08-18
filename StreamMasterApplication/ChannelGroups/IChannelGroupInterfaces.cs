using Microsoft.AspNetCore.Mvc;

using StreamMasterApplication.ChannelGroups.Commands;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;

namespace StreamMasterApplication.ChannelGroups;

public interface IChannelGroupController
{
    Task<ActionResult<List<string>>> GetChannelGroupNames();

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
    Task<List<string>> GetChannelGroupNames();
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