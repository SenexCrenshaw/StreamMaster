using StreamMaster.Domain.API;
using StreamMaster.Domain.Pagination;

namespace StreamMaster.Domain.Repository;
/// <summary>
/// Defines the contract for the channel group repository.
/// </summary>
public interface IChannelGroupRepository : IRepositoryBase<ChannelGroup>
{
    PagedResponse<ChannelGroupDto> CreateEmptyPagedResponse();

    // Queries returning single items

    /// <summary>
    /// Retrieves a channel group by its name.
    /// </summary>
    Task<ChannelGroup?> GetChannelGroupById(int channelGroupId);

    Task<ChannelGroup?> GetChannelGroupByName(string name);

    /// <summary>
    /// Retrieves channel groups based on specific parameters.
    /// </summary>
    Task<List<ChannelGroup>> GetChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves channel groups associated with specific M3U channel group names.
    /// </summary>
    Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames);

    /// <summary>
    /// Retrieves all channel group names.
    /// </summary>
    Task<List<ChannelGroupIdName>> GetChannelGroupNames(CancellationToken cancellationToken = default);

    Task<List<ChannelGroup>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves paginated channel groups based on specific parameters.
    /// </summary>
    Task<PagedResponse<ChannelGroup>> GetPagedChannelGroups(QueryStringParameters Parameters);

    // Commands or actions

    /// <summary>
    /// Creates a new channel group.
    /// </summary>
    Task<APIResponse> CreateChannelGroup(string GroupName, bool IsReadOnly = false);
    Task<APIResponse> CreateChannelGroups(List<string> GroupNames, bool IsReadOnly = false);

    /// <summary>
    /// Updates an existing channel group.
    /// </summary>
    void UpdateChannelGroup(ChannelGroup ChannelGroup);

    // Complex queries or commands with multiple return values

    /// <summary>
    /// Deletes multiple channel groups based on specific parameters and returns associated video streams.
    /// </summary>
    Task<APIResponse> DeleteAllChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken);

    Task<APIResponse> DeleteChannelGroupsRequest(List<int> channelGroupIds);

    Task<APIResponse> DeleteChannelGroupsByNameRequest(List<string> channelGroupNames);
}
