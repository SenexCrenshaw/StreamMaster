using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;

public interface IChannelGroupRepository : IRepositoryBase<ChannelGroup>
{
    bool DoesChannelGroupExist(string name);
    Task<APIResponse> CreateChannelGroup(string GroupName, bool IsReadOnly = false);
    Task<APIResponse> CreateChannelGroups(List<string> GroupNames, bool IsReadOnly = false);
    PagedResponse<ChannelGroupDto> CreateEmptyPagedResponse();
    Task<APIResponse> DeleteAllChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);
    Task<APIResponse> DeleteChannelGroupsByNameRequest(List<string> channelGroupNames);
    Task<APIResponse> DeleteChannelGroupsRequest(List<int> channelGroupIds);
    Task<ChannelGroup?> GetChannelGroupById(int channelGroupId);
    Task<ChannelGroup?> GetChannelGroupByName(string name);
    Task<List<ChannelGroup>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken);
    Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames);
    Task<List<ChannelGroup>> GetChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);
    Task<PagedResponse<ChannelGroup>> GetPagedChannelGroups(QueryStringParameters Parameters);
    void UpdateChannelGroup(ChannelGroup ChannelGroup);
}
