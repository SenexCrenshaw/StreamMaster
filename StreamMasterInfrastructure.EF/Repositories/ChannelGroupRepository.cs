using AutoMapper;
using AutoMapper.QueryableExtensions;

using EFCore.BulkExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Linq.Dynamic.Core;

namespace StreamMasterInfrastructureEF.Repositories;

public class ChannelGroupRepository(ILogger<ChannelGroupRepository> logger, RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, IMemoryCache memoryCache, ISender sender) : RepositoryBase<ChannelGroup>(repositoryContext, logger), IChannelGroupRepository
{

    /// <summary>
    /// Retrieves all channel groups from the database.
    /// </summary>
    /// <returns>A list of channel groups in their DTO representation.</returns>
    public async Task<List<ChannelGroupDto>> GetChannelGroups()
    {
        try
        {
            List<ChannelGroupDto> channelGroups = await FindAll().ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);

            channelGroups = await UpdateChannelGroupsWithActives(channelGroups);

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
    public async Task<List<ChannelGroupIdName>> GetChannelGroupNames()
    {
        try
        {
            List<ChannelGroupIdName> channelGroupNames = await FindAll()
                                        .OrderBy(channelGroup => channelGroup.Name)
                                        .Select(channelGroup => new ChannelGroupIdName
                                        {
                                            Name = channelGroup.Name,
                                            Id = channelGroup.Id
                                        })
                                        .ToListAsync()
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
    /// A PagedResponse containing the ChannelGroupDto objects enriched with count data from the memory cache.
    /// </returns>
    /// <exception cref="Exception">Throws any exception that occurs during the operation for higher layers to handle.</exception>
    public async Task<PagedResponse<ChannelGroupDto>> GetPagedChannelGroups(ChannelGroupParameters Parameters)
    {
        try
        {
            // Get IQueryable based on provided parameters
            IQueryable<ChannelGroup> query = GetIQueryableForEntity(Parameters);

            // Convert the IQueryable into a paged DTO response
            PagedResponse<ChannelGroupDto> ret = await query.GetPagedResponseAsync<ChannelGroup, ChannelGroupDto>(Parameters.PageNumber, Parameters.PageSize, mapper);

            // Get channel group stream counts from the memory cache
            IEnumerable<ChannelGroupStreamCount> actives = memoryCache.ChannelGroupStreamCounts();

            // Enrich the DTOs with the counts from the memory cache
            foreach (ChannelGroupStreamCount? active in actives)
            {
                ChannelGroupDto? dto = ret.Data.FirstOrDefault(a => a.Id == active.ChannelGroupId);
                if (dto != null)
                {
                    dto.ActiveCount = active.ActiveCount;
                    dto.HiddenCount = active.HiddenCount;
                    dto.TotalCount = active.TotalCount;
                }
            }

            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching and enriching the ChannelGroups.");
            throw;
        }
    }


    private async Task<ChannelGroupDto?> GetChannelGroupFromVideoStream(string channelGroupName)
    {
        ChannelGroupDto? ret = await GetChannelGroupByName(channelGroupName).ConfigureAwait(false);
        ret = UpdateChannelGroupWithActives(ret);

        return ret;
    }

    public async Task<string?> GetChannelGroupNameFromVideoStream(string videoStreamId)
    {

        VideoStream? videoStream = await RepositoryContext.VideoStreams.FirstOrDefaultAsync(a => a.Id == videoStreamId).ConfigureAwait(false);

        if (videoStream == null)
        {
            return null;
        }

        return videoStream.User_Tvg_name;
    }

    /// <summary>
    /// Retrieves a ChannelGroupDto based on a VideoStream's ID.
    /// </summary>
    /// <param name="VideoStreamId">The ID of the VideoStream.</param>
    /// <param name="cancellationToken">Token to support task cancellation.</param>
    /// <returns>A ChannelGroupDto associated with the VideoStream, or null if not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided VideoStreamId is null or empty.</exception>
    public async Task<ChannelGroupDto?> GetChannelGroupFromVideoStreamId(string videoStreamId)
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

        logger.LogInformation($"Fetching ChannelGroupDto for VideoStream with group: {videoStream.User_Tvg_group}.");
        ChannelGroupDto? channelGroup = await GetChannelGroupFromVideoStream(videoStream.User_Tvg_group).ConfigureAwait(false);
        channelGroup = UpdateChannelGroupWithActives(channelGroup);
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

        logger.LogInformation($"Fetching ChannelGroupDto for VideoStream with group name: {channelGroupName}.");

        ChannelGroupDto? channelGroup = await GetChannelGroupFromVideoStream(channelGroupName).ConfigureAwait(false);

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
    public async Task<ChannelGroupDto?> GetChannelGroupById(int Id)
    {
        if (Id <= 0)
        {
            logger.LogError($"Invalid Id provided: {Id}. Id should be a positive integer.");
            throw new ArgumentException("Value should be a positive integer.", nameof(Id));
        }

        logger.LogInformation($"Attempting to fetch ChannelGroup with Id: {Id}.");

        ChannelGroup? channelGroup = await FindByCondition(c => c.Id == Id)
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync();

        if (channelGroup == null)
        {
            logger.LogWarning($"No ChannelGroup found with Id: {Id}.");
        }
        ChannelGroupDto? ret = mapper.Map<ChannelGroupDto?>(channelGroup);
        ret = UpdateChannelGroupWithActives(ret);

        return ret;
    }


    //public async Task<ChannelGroupDto?> GetChannelGroupAsync(int Id, CancellationToken cancellationToken = default)
    //{
    //    ChannelGroup? res = await FindByCondition(channelGroup => channelGroup.Id == Id).FirstOrDefaultAsync();
    //    if (res == null)
    //    {
    //        return null;
    //    }
    //    ChannelGroupDto dtos = mapper.Map<ChannelGroupDto>(res);

    //    return dtos;
    //}

    /// <summary>
    /// Retrieves a list of ChannelGroupDto objects based on a list of channel group names.
    /// </summary>
    /// <param name="m3uChannelGroupNames">A list of channel group names to be used as a filter.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a list of ChannelGroupDto objects that match the provided channel group names.
    /// Returns an empty list if no matches are found.
    /// </returns>
    /// <exception cref="Exception">Throws any exceptions that arise during data retrieval or mapping.</exception>
    public async Task<List<ChannelGroupDto>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames)
    {
        try
        {
            // Fetching matching ChannelGroup entities based on provided names
            IQueryable<ChannelGroup> query = FindByCondition(channelGroup => m3uChannelGroupNames.Contains(channelGroup.Name));

            // Asynchronously retrieving results and mapping to DTOs
            List<ChannelGroupDto> channelGroups = await query.ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider).ToListAsync();
            channelGroups = await UpdateChannelGroupsWithActives(channelGroups);
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
    /// Retrieves a ChannelGroup entity based on its name and maps it to a ChannelGroupDto.
    /// </summary>
    /// <param name="name">The name of the ChannelGroup to retrieve.</param>
    /// <returns>
    /// A ChannelGroupDto representation of the ChannelGroup if found; otherwise, null.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided name is null or whitespace.</exception>
    /// <exception cref="Exception">Throws any exception that occurs during the operation for higher layers to handle.</exception>
    public async Task<ChannelGroupDto?> GetChannelGroupByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogError("The provided name for ChannelGroup was null or whitespace.");
            throw new ArgumentNullException(nameof(name));
        }

        try
        {
            ChannelGroup? channelGroup = await FindByCondition(channelGroup => channelGroup.Name.Equals(name)).FirstOrDefaultAsync();
            if (channelGroup == null)
            {
                logger.LogInformation("No ChannelGroup found with the name {Name}.", name);
            }

            ChannelGroupDto? ret = mapper.Map<ChannelGroupDto?>(channelGroup);
            ret = UpdateChannelGroupWithActives(ret);

            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching the ChannelGroup with name {Name}.", name);
            throw;
        }
    }




    //public async Task<IEnumerable<ChannelGroup>> GetAllChannelGroupsAsync()
    //{
    //    return await FindAll()
    //                    .OrderBy(p => p.Id)
    //                    .ToListAsync();
    //}

    /// <summary>
    /// Creates a new channel group in the database.
    /// </summary>
    /// <param name="channelGroup">The channel group to be added.</param>
    public void CreateChannelGroup(ChannelGroup channelGroup)
    {
        if (channelGroup == null)
        {
            logger.LogError("Attempted to create a null channel group.");
            throw new ArgumentNullException(nameof(channelGroup), "ChannelGroup cannot be null.");
        }

        try
        {
            Create(channelGroup);
            logger.LogInformation($"Successfully created channel group with ID: {channelGroup.Id}");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error creating channel group: {ex.Message}", ex);
            throw;  // or handle the exception accordingly
        }
    }


    public async Task<(int? ChannelGroupId, List<VideoStreamDto> VideoStreams)> DeleteChannelGroupById(int ChannelGroupId)
    {
        ChannelGroup? channelGroup = await FindByCondition(a => a.Id == ChannelGroupId).FirstOrDefaultAsync().ConfigureAwait(false);
        if (channelGroup == null)
        {
            return (null, new List<VideoStreamDto>());
        }
        return await DeleteChannelGroup(channelGroup).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes the specified ChannelGroup and updates associated video streams.
    /// </summary>
    /// <param name="channelGroup">The ChannelGroup to delete.</param>
    /// <returns>A tuple containing the deleted ChannelGroup's ID and the updated video streams.</returns>
    public async Task<(int? ChannelGroupId, List<VideoStreamDto> VideoStreams)> DeleteChannelGroup(ChannelGroup channelGroup)
    {
        logger.LogInformation($"Preparing to delete ChannelGroup with ID {channelGroup.Id} and update associated video streams.");

        // Fetching associated video streams for the ChannelGroup
        List<VideoStreamDto> videoStreams = await sender.Send(new GetVideoStreamsForChannelGroups(new List<int> { channelGroup.Id })).ConfigureAwait(false);

        // Deleting the ChannelGroup
        Delete(channelGroup);

        // Extracting the IDs of the fetched video streams
        IEnumerable<string> videoStreamIds = videoStreams.Select(a => a.Id);

        await repository.VideoStream.UpdateVideoStreamsChannelGroupNames(videoStreamIds, "").ConfigureAwait(false);

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
    public void UpdateChannelGroup(ChannelGroup channelGroup)
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
    /// Fetches a list of ChannelGroupDto based on the provided ChannelGroupParameters.
    /// </summary>
    /// <param name="Parameters">The parameters based on which ChannelGroupDto are fetched.</param>
    /// <param name="cancellationToken">Token to support task cancellation.</param>
    /// <returns>A list of ChannelGroupDto matching the provided parameters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided Parameters are null.</exception>
    public async Task<List<ChannelGroupDto>> GetChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken)
    {
        if (Parameters == null)
        {
            logger.LogError("GetChannelGroupsFromParameters failed - Parameters are null.");
            throw new ArgumentNullException(nameof(Parameters));
        }

        logger.LogInformation($"Fetching ChannelGroupDto based on provided parameters.");

        IQueryable<ChannelGroup> queryable = GetIQueryableForEntity(Parameters);

        List<ChannelGroupDto> result = await queryable.ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken).ConfigureAwait(false);
        result = await UpdateChannelGroupsWithActives(result);

        logger.LogInformation($"{result.Count} ChannelGroupDto entries fetched based on provided parameters.");

        return result;
    }


    /// <summary>
    /// Deletes all ChannelGroups based on the provided parameters and returns the associated video streams.
    /// </summary>
    /// <param name="Parameters">The filter parameters for determining which ChannelGroups to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing a list of deleted ChannelGroup IDs and associated video streams.</returns>
    public async Task<(List<int> ChannelGroupIds, List<VideoStreamDto> VideoStreams)>
            DeleteAllChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to fetch and delete ChannelGroups based on provided parameters.");

        IQueryable<ChannelGroup> toDeleteQuery = GetIQueryableForEntity(Parameters).Where(a => !a.IsReadOnly);

        List<int> channelGroupIds = await toDeleteQuery.Select(a => a.Id).ToListAsync(cancellationToken).ConfigureAwait(false);

        List<VideoStreamDto> videoStreams = await sender.Send(new GetVideoStreamsForChannelGroups(channelGroupIds), cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"Preparing to bulk delete {channelGroupIds.Count} ChannelGroups.");

        await RepositoryContext.BulkDeleteAsync(toDeleteQuery, cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"Successfully deleted {channelGroupIds.Count} ChannelGroups.");

        return (channelGroupIds, videoStreams);
    }


    public async Task<List<ChannelGroupDto>> GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = RepositoryContext.VideoStreams.Where(a => VideoStreamIds.Contains(a.Id));

        if (!videoStreams.Any())
        {
            return new List<ChannelGroupDto>();
        }

        IQueryable<string> channeNames = videoStreams.Select(a => a.User_Tvg_group).Distinct();
        List<ChannelGroupDto> ret = await FindByCondition(a => channeNames.Contains(a.Name)).ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        ret = await UpdateChannelGroupsWithActives(ret);

        return ret;
    }

    public IQueryable<ChannelGroup> GetChannelGroupQuery()
    {
        return FindAll();
    }

    public async Task<List<ChannelGroupDto>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken)
    {
        List<ChannelGroupDto> channelGroups = await FindAll().ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);

        List<int> selectedIds = await RepositoryContext.StreamGroupChannelGroups.Where(a => a.StreamGroupId == streamGroupId).Select(a => a.ChannelGroupId).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        channelGroups = await UpdateChannelGroupsWithActives(channelGroups).ConfigureAwait(false);

        channelGroups = channelGroups
        .OrderBy(a => selectedIds.Contains(a.Id) ? 0 : 1)
        .ThenBy(a => a.Name)
        .ToList();

        return channelGroups;
    }

    private ChannelGroupDto? UpdateChannelGroupWithActives(ChannelGroupDto? channelGroup)
    {
        if (channelGroup == null)
        {
            return null;
        }
        ChannelGroupStreamCount? active = memoryCache.ChannelGroupStreamCounts().FirstOrDefault(a => a.ChannelGroupId == channelGroup.Id);
        if (active == null)
        {
            return channelGroup;
        }

        channelGroup.ActiveCount = active.ActiveCount;
        channelGroup.HiddenCount = active.HiddenCount;
        channelGroup.TotalCount = active.TotalCount;

        return channelGroup;
    }
    private async Task<List<ChannelGroupDto>> UpdateChannelGroupsWithActives(List<ChannelGroupDto> channelGroups)
    {

        foreach (ChannelGroupDto channelGroup in channelGroups)
        {
            UpdateChannelGroupWithActives(channelGroup);
        }
        return channelGroups;
    }

    //public PagedResponse<ChannelGroupDto> CreateEmptyPagedResponse(ChannelGroupParameters parameters)
    //{
    //    return CreateEmptyPagedResponse<ChannelGroupDto>(parameters);
    //}
}