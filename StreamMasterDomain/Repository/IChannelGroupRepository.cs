using StreamMasterDomain.Dto;
using StreamMasterDomain.Models;
using StreamMasterDomain.Pagination;

namespace StreamMasterDomain.Repository;
/// <summary>
/// Defines the contract for the channel group repository.
/// </summary>
public interface IChannelGroupRepository : IRepositoryBase<ChannelGroup>
{
    // Queries returning single items

    /// <summary>
    /// Retrieves a channel group by its name.
    /// </summary>
    Task<ChannelGroupDto?> GetChannelGroupByName(string name);

    /// <summary>
    /// Retrieves a channel group by its ID.
    /// </summary>
    Task<ChannelGroupDto?> GetChannelGroupById(int Id);

    /// <summary>
    /// Retrieves a channel group associated with a specific video stream ID.
    /// </summary>
    Task<ChannelGroupDto?> GetChannelGroupFromVideoStreamId(string VideoStreamId);

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
    Task<List<ChannelGroupDto>> GetChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves channel groups associated with specific M3U channel group names.
    /// </summary>
    Task<List<ChannelGroupDto>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames);

    /// <summary>
    /// Retrieves all channel group names.
    /// </summary>
    Task<List<ChannelGroupIdName>> GetChannelGroupNames();

    Task<List<ChannelGroupDto>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all channel groups.
    /// </summary>
    Task<List<ChannelGroupDto>> GetChannelGroups();

    IQueryable<ChannelGroup> GetChannelGroupQuery();

    /// <summary>
    /// Retrieves channel groups associated with specific video stream IDs.
    /// </summary>
    Task<List<ChannelGroupDto>> GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves paginated channel groups based on specific parameters.
    /// </summary>
    Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(ChannelGroupParameters Parameters);

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
