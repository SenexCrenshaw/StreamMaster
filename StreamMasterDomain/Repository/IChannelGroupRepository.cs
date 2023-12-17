using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;
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
    Task<ChannelGroup?> GetChannelGroupByName(string name);

    /// <summary>
    /// Retrieves a channel group by its ID.
    /// </summary>
    Task<ChannelGroup?> GetChannelGroupById(int Id);

    /// <summary>
    /// Retrieves a channel group associated with a specific video stream ID.
    /// </summary>
    Task<ChannelGroup?> GetChannelGroupFromVideoStreamId(string VideoStreamId);

    /// <summary>
    /// Retrieves a channel group name associated with a specific video stream.
    /// </summary>
    Task<string?> GetChannelGroupNameFromVideoStream(string videoStreamId);

    /// <summary>
    /// Retrieves a channel group ID associated with a channel group name.
    /// </summary>
    Task<int?> GetChannelGroupIdFromVideoStream(string channelGroupName);

    // Queries returning lists or collections

    /// <summary>
    /// Retrieves channel groups based on specific parameters.
    /// </summary>
    Task<List<ChannelGroup>> GetChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves channel groups associated with specific M3U channel group names.
    /// </summary>
    Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames);

    /// <summary>
    /// Retrieves all channel group names.
    /// </summary>
    Task<List<ChannelGroupIdName>> GetChannelGroupNames(CancellationToken cancellationToken);

    Task<List<ChannelGroup>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all channel groups.
    /// </summary>
    Task<List<ChannelGroup>> GetChannelGroups(List<int>? ids = null);


    //IQueryable<ChannelGroup> GetChannelGroupQuery();

    /// <summary>
    /// Retrieves channel groups associated with specific video stream IDs.
    /// </summary>
    Task<List<ChannelGroup>> GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves paginated channel groups based on specific parameters.
    /// </summary>
    Task<PagedResponse<ChannelGroup>> GetPagedChannelGroups(ChannelGroupParameters Parameters);

    // Commands or actions

    /// <summary>
    /// Creates a new channel group.
    /// </summary>
    void CreateChannelGroup(ChannelGroup ChannelGroup);

    /// <summary>
    /// Updates an existing channel group.
    /// </summary>
    void UpdateChannelGroup(ChannelGroup ChannelGroup);

    // Complex queries or commands with multiple return values

    /// <summary>
    /// Deletes multiple channel groups based on specific parameters and returns associated video streams.
    /// </summary>
    Task<(List<int> ChannelGroupIds, List<VideoStreamDto> VideoStreams)> DeleteAllChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a specific channel group and returns associated video streams.
    /// </summary>
    Task<(int? ChannelGroupId, List<VideoStreamDto> VideoStreams)> DeleteChannelGroupById(int ChannelGroupId);
}
