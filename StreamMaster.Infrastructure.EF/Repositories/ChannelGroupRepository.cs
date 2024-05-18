using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.VideoStreams.Queries;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;

using System.Linq.Dynamic.Core;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class ChannelGroupRepository(
    ILogger<ChannelGroupRepository> intLogger,
    IRepositoryContext intRepositoryContext,
    IRepositoryWrapper Repository,
    IMapper Mapper,
    IOptionsMonitor<Setting> intSettings,
    ISender sender
    ) : RepositoryBase<Domain.Models.ChannelGroup>(intRepositoryContext, intLogger, intSettings), IChannelGroupRepository
{

    public async Task<ChannelGroupDto?> CreateChannelGroup(string GroupName, bool IsReadOnly = false)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByName(GroupName).ConfigureAwait(false) != null)
        {
            return null;
        }

        Domain.Models.ChannelGroup channelGroup = new() { Name = GroupName, IsReadOnly = IsReadOnly };
        Create(channelGroup); ;
        await SaveChangesAsync();

        List<StreamGroupDto>? ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroupByChannelId(channelGroup.Id);

        ChannelGroupDto channelGroupDto = Mapper.Map<ChannelGroupDto>(channelGroup);
        return channelGroupDto;
    }

    /// <summary>
    /// Retrieves all channel groups from the database.
    /// </summary>
    /// <returns>A list of channel groups in their DTO representation.</returns>
    public async Task<List<Domain.Models.ChannelGroup>> GetChannelGroups(List<int>? ids = null)
    {
        try
        {
            List<Domain.Models.ChannelGroup> channelGroups = [];

            IQueryable<Domain.Models.ChannelGroup> query = base.GetQuery();
            channelGroups = ids == null
                ? await GetQuery().ToListAsync().ConfigureAwait(false)
                : await GetQuery(a => ids.Contains(a.Id)).ToListAsync().ConfigureAwait(false);

            logger.LogInformation($"Successfully retrieved {channelGroups.Count} channel groups.");

            return channelGroups;
        }
        catch (Exception ex)
        {
            logger.LogError($"Error retrieving all channel groups: {ex.Message}", ex);
            throw; // or handle the exception accordingly
        }
    }

    /// <summary>
    /// Asynchronously retrieves a list of all channel group names, ordered by name.
    /// </summary>
    /// <returns>A list of channel group names and their corresponding IDs.</returns>
    public async Task<List<ChannelGroupIdName>> GetChannelGroupNames(CancellationToken cancellationToken)
    {
        try
        {

            List<ChannelGroupIdName> channelGroupNames = await RepositoryContext.ChannelGroups
             .OrderBy(channelGroup => channelGroup.Name)
             .Select(channelGroup => new ChannelGroupIdName
             {
                 Name = channelGroup.Name,
                 Id = channelGroup.Id,
                 TotalCount = RepositoryContext.VideoStreams
                              .Count(vs => vs.User_Tvg_group == channelGroup.Name)
             })
             .ToListAsync(cancellationToken)
             .ConfigureAwait(false);

            return channelGroupNames;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to retrieve channel group names: {ex.Message}", ex);
            throw;  // or return a default/fallback value, depending on your application's behavior
        }
    }

    /// <summary>
    /// Fetches paged ChannelGroup entities based on provided parameters, maps them to their DTO representations,
    /// and then enriches each DTO with additional counts from a memory cache.
    /// </summary>
    /// <param name="channelGroupParameters">The parameters to filter and page the ChannelGroup entities.</param>
    /// <returns>
    /// A PagedResponse containing the ChannelGroup objects enriched with count data from the memory cache.
    /// </returns>
    /// <exception cref="Exception">Throws any exception that occurs during the operation for higher layers to handle.</exception>
    public async Task<PagedResponse<Domain.Models.ChannelGroup>> GetPagedChannelGroups(QueryStringParameters Parameters)
    {
        try
        {
            // Get IQueryable based on provided parameters
            IQueryable<Domain.Models.ChannelGroup> query = GetQuery(Parameters);

            PagedResponse<Domain.Models.ChannelGroup> ret = await query.GetPagedResponseAsync(Parameters.PageNumber, Parameters.PageSize);

            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching and enriching the ChannelGroups.");
            throw;
        }
    }

    private async Task<Domain.Models.ChannelGroup?> GetChannelGroupFromVideoStream(string channelGroupName)
    {
        Domain.Models.ChannelGroup? ret = await GetChannelGroupByName(channelGroupName).ConfigureAwait(false);

        return ret;
    }

    public async Task<string?> GetChannelGroupNameFromVideoStream(string videoStreamId)
    {
        VideoStream? videoStream = await RepositoryContext.VideoStreams.FirstOrDefaultAsync(a => a.Id == videoStreamId).ConfigureAwait(false);

        return videoStream?.User_Tvg_name;
    }

    /// <summary>
    /// Retrieves a ChannelGroup based on a VideoStream's ID.
    /// </summary>
    /// <param name="VideoStreamId">The ID of the VideoStream.</param>
    /// <param name="cancellationToken">Token to support task cancellation.</param>
    /// <returns>A ChannelGroup associated with the VideoStream, or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided CurrentVideoStreamId is null or empty.</exception>
    public async Task<Domain.Models.ChannelGroup?> GetChannelGroupFromVideoStreamId(string videoStreamId)
    {
        if (string.IsNullOrEmpty(videoStreamId))
        {
            logger.LogError("GetChannelGroupFromVideoStreamId failed - VideoStreamId is null or empty.");
            throw new ArgumentException("Value cannot be null or empty.", nameof(videoStreamId));
        }

        logger.LogInformation($"Fetching VideoStream with ID: {videoStreamId}.");

        VideoStream? videoStream = await RepositoryContext.VideoStreams.FirstOrDefaultAsync(a => a.Id == videoStreamId);

        if (videoStream == null)
        {
            logger.LogWarning($"No VideoStream found with ID: {videoStreamId}.");
            return null;
        }

        logger.LogInformation($"Fetching ChannelGroup for VideoStream with group: {videoStream.User_Tvg_group}.");
        Domain.Models.ChannelGroup? channelGroup = await GetChannelGroupFromVideoStream(videoStream.User_Tvg_group).ConfigureAwait(false);

        return channelGroup;
    }

    /// <summary>
    /// Retrieves the ID of a ChannelGroup based on its associated VideoStream's name.
    /// </summary>
    /// <param name="channelGroupName">The name of the channel group associated with the VideoStream.</param>
    /// <returns>The ID of the ChannelGroup, or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided channelGroupName is null or empty.</exception>
    public async Task<int?> GetChannelGroupIdFromVideoStream(string channelGroupName)
    {
        if (string.IsNullOrEmpty(channelGroupName))
        {
            logger.LogError("GetChannelGroupIdFromVideoStream failed - channelGroupName is null or empty.");
            throw new ArgumentException("Value cannot be null or empty.", nameof(channelGroupName));
        }

        logger.LogInformation($"Fetching ChannelGroup for VideoStream with group name: {channelGroupName}.");

        Domain.Models.ChannelGroup? channelGroup = await GetChannelGroupFromVideoStream(channelGroupName).ConfigureAwait(false);

        if (channelGroup == null)
        {
            logger.LogWarning($"No ChannelGroup found with group name: {channelGroupName}.");
        }

        return channelGroup?.Id;
    }

    /// <summary>
    /// Retrieves a ChannelGroup DTO based on its identifier.
    /// </summary>
    /// <param name="Id">The identifier of the desired ChannelGroup.</param>
    /// <returns>A ChannelGroup DTO if found; otherwise, null.</returns>
    public async Task<Domain.Models.ChannelGroup?> GetChannelGroupById(int Id)
    {
        if (Id <= 0)
        {
            logger.LogError($"Invalid Id provided: {Id}. Id should be a positive integer.");
            throw new ArgumentException("Value should be a positive integer.", nameof(Id));
        }

        logger.LogInformation($"Attempting to fetch ChannelGroup with Id: {Id}.");

        Domain.Models.ChannelGroup? channelGroup = await FirstOrDefaultAsync(c => c.Id == Id);

        if (channelGroup == null)
        {
            logger.LogWarning($"No ChannelGroup found with Id: {Id}.");
        }

        return channelGroup;
    }

    /// <summary>
    /// Retrieves a list of ChannelGroup objects based on a list of channel group names.
    /// </summary>
    /// <param name="m3uChannelGroupNames">A list of channel group names to be used as a filter.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a list of ChannelGroup objects that match the provided channel group names.
    /// Returns an empty list if no matches are found.
    /// </returns>
    /// <exception cref="Exception">Throws any exceptions that arise during data retrieval or mapping.</exception>
    public async Task<List<Domain.Models.ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames)
    {
        try
        {
            // Fetching matching ChannelGroup entities based on provided names
            IQueryable<Domain.Models.ChannelGroup> query = base.GetQuery(channelGroup => m3uChannelGroupNames.Contains(channelGroup.Name));

            // Asynchronously retrieving results and mapping to DTOs
            List<Domain.Models.ChannelGroup> channelGroups = await query.ToListAsync().ConfigureAwait(false);

            return channelGroups;
        }
        catch (Exception ex)
        {
            // Logging any exceptions that occur and re-throwing
            logger.LogError(ex, "An error occurred while fetching ChannelGroups by names.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a ChannelGroup entity based on its name and maps it to a ChannelGroup.
    /// </summary>
    /// <param name="name">The name of the ChannelGroup to retrieve.</param>
    /// <returns>
    /// A ChannelGroup representation of the ChannelGroup if found; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided name is null or whitespace.</exception>
    /// <exception cref="Exception">Throws any exception that occurs during the operation for higher layers to handle.</exception>
    public async Task<Domain.Models.ChannelGroup?> GetChannelGroupByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogError("The provided name for ChannelGroup was null or whitespace.");
            return null;
        }

        try
        {
            Domain.Models.ChannelGroup? channelGroup = await FirstOrDefaultAsync(channelGroup => channelGroup.Name.Equals(name));
            return channelGroup ?? null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching the ChannelGroup with name {Name}.", name);
            throw;
        }
    }

    //public async Task<IEnumerable<ChannelGroup>> GetAllChannelGroupsAsync()
    //{
    //    return await GetQuery()
    //                    .OrderBy(p => p.Id)
    //                    .ToListAsync();
    //}



    public async Task<(int? ChannelGroupId, List<VideoStreamDto> VideoStreams)> DeleteChannelGroupById(int ChannelGroupId)
    {
        Domain.Models.ChannelGroup? channelGroup = await FirstOrDefaultAsync(a => a.Id == ChannelGroupId).ConfigureAwait(false);
        return channelGroup == null ? ((int? ChannelGroupId, List<VideoStreamDto> VideoStreams))(null, []) : await DeleteChannelGroup(channelGroup).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the specified ChannelGroup and updates associated video streams.
    /// </summary>
    /// <param name="channelGroup">The ChannelGroup to delete.</param>
    /// <returns>A tuple containing the deleted ChannelGroup's ID and the updated video streams.</returns>
    public async Task<(int? ChannelGroupId, List<VideoStreamDto> VideoStreams)> DeleteChannelGroup(Domain.Models.ChannelGroup channelGroup)
    {
        logger.LogInformation($"Preparing to delete ChannelGroup with ID {channelGroup.Id} and update associated video streams.");

        // Fetching associated video streams for the ChannelGroup
        List<VideoStreamDto> videoStreams = await sender.Send(new GetVideoStreamsForChannelGroups([channelGroup.Id])).ConfigureAwait(false);

        // Deleting the ChannelGroup
        Delete(channelGroup);

        // Extracting the IDs of the fetched video streams
        IEnumerable<string> videoStreamIds = videoStreams.Select(a => a.Id);

        await Repository.VideoStream.UpdateVideoStreamsChannelGroupNames(videoStreamIds, "").ConfigureAwait(false);

        logger.LogInformation($"Updated associated video streams for ChannelGroup with ID {channelGroup.Id}.");

        // Update the User_Tvg_group of videoStreams in memory after the database update
        foreach (VideoStreamDto item in videoStreams)
        {
            item.User_Tvg_group = "";
        }

        logger.LogInformation($"Successfully deleted ChannelGroup with ID {channelGroup.Id} and updated associated video streams.");

        return (channelGroup.Id, videoStreams);
    }

    /// <summary>
    /// Updates the provided ChannelGroup entity.
    /// </summary>
    /// <param name="ChannelGroup">The ChannelGroup entity to be updated.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided ChannelGroup is null.</exception>
    public void UpdateChannelGroup(Domain.Models.ChannelGroup channelGroup)
    {
        if (channelGroup == null)
        {
            logger.LogError("UpdateChannelGroup failed - ChannelGroup is null.");
            throw new ArgumentNullException(nameof(channelGroup));
        }

        logger.LogInformation($"Updating ChannelGroup with ID {channelGroup.Id}.");
        Update(channelGroup);
    }

    /// <summary>
    /// Fetches a list of ChannelGroup based on the provided ChannelGroupParameters.
    /// </summary>
    /// <param name="Parameters">The parameters based on which ChannelGroup are fetched.</param>
    /// <param name="cancellationToken">Token to support task cancellation.</param>
    /// <returns>A list of ChannelGroup matching the provided parameters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided Parameters are null.</exception>
    public async Task<List<Domain.Models.ChannelGroup>> GetChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken)
    {
        if (Parameters == null)
        {
            logger.LogError("GetChannelGroupsFromParameters failed - Parameters are null.");
            throw new ArgumentNullException(nameof(Parameters));
        }

        logger.LogInformation($"Fetching ChannelGroup based on provided parameters.");

        IQueryable<Domain.Models.ChannelGroup> queryable = GetQuery(Parameters);

        List<Domain.Models.ChannelGroup> result = await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"{result.Count} ChannelGroup entries fetched based on provided parameters.");

        return result;
    }

    /// <summary>
    /// Deletes all ChannelGroups based on the provided parameters and returns the associated video streams.
    /// </summary>
    /// <param name="Parameters">The filter parameters for determining which ChannelGroups to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing a list of deleted ChannelGroup IDs and associated video streams.</returns>
    public async Task<(List<int> ChannelGroupIds, List<VideoStreamDto> VideoStreams)> DeleteAllChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to fetch and delete ChannelGroups based on provided parameters.");

        IQueryable<Domain.Models.ChannelGroup> toDeleteQuery = GetQuery(Parameters).Where(a => !a.IsReadOnly);

        List<int> channelGroupIds = await toDeleteQuery.Select(a => a.Id).ToListAsync(cancellationToken).ConfigureAwait(false);

        List<VideoStreamDto> videoStreams = await Repository.VideoStream.GetVideoStreamsForChannelGroups(channelGroupIds, cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"Preparing to bulk delete {channelGroupIds.Count} ChannelGroups.");

        await RepositoryContext.BulkDeleteAsyncEntities(toDeleteQuery, cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"Successfully deleted {channelGroupIds.Count} ChannelGroups.");

        return (channelGroupIds, videoStreams);
    }

    public async Task<List<Domain.Models.ChannelGroup>> GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = RepositoryContext.VideoStreams.Where(a => VideoStreamIds.Contains(a.Id));

        if (!videoStreams.Any())
        {
            return [];
        }

        IQueryable<string> channeNames = videoStreams.Select(a => a.User_Tvg_group).Distinct();
        List<Domain.Models.ChannelGroup> ret = await base.GetQuery(a => channeNames.Contains(a.Name)).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        return ret;
    }

    public IQueryable<Domain.Models.ChannelGroup> GetChannelGroupQuery()
    {
        return GetQuery();
    }

    public async Task<List<Domain.Models.ChannelGroup>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken)
    {
        List<Domain.Models.ChannelGroup> channelGroups = await base.GetQuery().ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        List<int> selectedIds = await RepositoryContext.StreamGroupChannelGroups.Where(a => a.StreamGroupId == streamGroupId).Select(a => a.ChannelGroupId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        channelGroups = channelGroups
        .OrderBy(a => selectedIds.Contains(a.Id) ? 0 : 1)
        .ThenBy(a => a.Name)
        .ToList();

        return channelGroups;
    }

    public PagedResponse<ChannelGroupDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<ChannelGroupDto>(Count());
    }
}