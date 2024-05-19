using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

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
    ) : RepositoryBase<ChannelGroup>(intRepositoryContext, intLogger, intSettings), IChannelGroupRepository
{

    public async Task<APIResponse> CreateChannelGroup(string GroupName, bool IsReadOnly = false)
    {
        if (await Repository.ChannelGroup.GetChannelGroupByName(GroupName).ConfigureAwait(false) != null)
        {
            return APIResponse.Ok;
        }

        ChannelGroup channelGroup = new() { Name = GroupName, IsReadOnly = IsReadOnly };
        Create(channelGroup); ;
        await SaveChangesAsync();

        List<StreamGroupDto>? ret = await Repository.StreamGroupChannelGroup.SyncStreamGroupChannelGroupByChannelId(channelGroup.Id);


        return APIResponse.Ok;
    }


    public async Task<APIResponse> CreateChannelGroups(List<string> GroupNames, bool IsReadOnly = false)
    {
        IQueryable<string> existingNames = GetQuery().Select(x => x.Name);
        IEnumerable<string> toCreate = GroupNames.Where(a => !existingNames.Contains(a));
        foreach (string name in toCreate)
        {
            Create(new ChannelGroup
            {
                Name = name,
                IsReadOnly = IsReadOnly
            });
        }
        return APIResponse.Ok;
    }


    /// <summary>
    /// Asynchronously retrieves a list of all channel group names, ordered by name.
    /// </summary>
    /// <returns>A list of channel group names and their corresponding IDs.</returns>
    public async Task<List<ChannelGroupIdName>> GetChannelGroupNames(CancellationToken cancellationToken = default)
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
    public async Task<PagedResponse<ChannelGroup>> GetPagedChannelGroups(QueryStringParameters Parameters)
    {
        try
        {
            // Get IQueryable based on provided parameters
            IQueryable<ChannelGroup> query = GetQuery(Parameters);

            PagedResponse<ChannelGroup> ret = await query.GetPagedResponseAsync(Parameters.PageNumber, Parameters.PageSize);

            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching and enriching the ChannelGroups.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a ChannelGroup DTO based on its identifier.
    /// </summary>
    /// <param name="Id">The identifier of the desired ChannelGroup.</param>
    /// <returns>A ChannelGroup DTO if found; otherwise, null.</returns>
    public async Task<ChannelGroup?> GetChannelGroupById(int channelGroupId)
    {
        if (channelGroupId <= 0)
        {
            logger.LogError($"Invalid Id provided: {channelGroupId}. Id should be a positive integer.");
            throw new ArgumentException("Value should be a positive integer.", nameof(channelGroupId));
        }

        logger.LogInformation($"Attempting to fetch ChannelGroup with Id: {channelGroupId}.");

        ChannelGroup? channelGroup = await FirstOrDefaultAsync(c => c.Id == channelGroupId);

        if (channelGroup == null)
        {
            logger.LogWarning($"No ChannelGroup found with Id: {channelGroupId}.");
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
    public async Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames)
    {
        try
        {
            // Fetching matching ChannelGroup entities based on provided names
            IQueryable<ChannelGroup> query = base.GetQuery(channelGroup => m3uChannelGroupNames.Contains(channelGroup.Name));

            // Asynchronously retrieving results and mapping to DTOs
            List<ChannelGroup> channelGroups = await query.ToListAsync().ConfigureAwait(false);

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
    public async Task<ChannelGroup?> GetChannelGroupByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            logger.LogError("The provided name for ChannelGroup was null or whitespace.");
            return null;
        }

        try
        {
            ChannelGroup? channelGroup = await FirstOrDefaultAsync(channelGroup => channelGroup.Name.Equals(name));
            return channelGroup ?? null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while fetching the ChannelGroup with name {Name}.", name);
            throw;
        }
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
    /// Fetches a list of ChannelGroup based on the provided ChannelGroupParameters.
    /// </summary>
    /// <param name="Parameters">The parameters based on which ChannelGroup are fetched.</param>
    /// <param name="cancellationToken">Token to support task cancellation.</param>
    /// <returns>A list of ChannelGroup matching the provided parameters.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided Parameters are null.</exception>
    public async Task<List<ChannelGroup>> GetChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken)
    {
        if (Parameters == null)
        {
            logger.LogError("GetChannelGroupsFromParameters failed - Parameters are null.");
            throw new ArgumentNullException(nameof(Parameters));
        }

        logger.LogInformation($"Fetching ChannelGroup based on provided parameters.");

        IQueryable<ChannelGroup> queryable = GetQuery(Parameters);

        List<ChannelGroup> result = await queryable.ToListAsync(cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"{result.Count} ChannelGroup entries fetched based on provided parameters.");

        return result;
    }

    /// <summary>
    /// Deletes all ChannelGroups based on the provided parameters and returns the associated video streams.
    /// </summary>
    /// <param name="Parameters">The filter parameters for determining which ChannelGroups to delete.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A tuple containing a list of deleted ChannelGroup IDs and associated video streams.</returns>
    public async Task<APIResponse> DeleteAllChannelGroupsFromParameters(QueryStringParameters Parameters, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to fetch and delete ChannelGroups based on provided parameters.");

        IQueryable<ChannelGroup> toDeleteQuery = GetQuery(Parameters).Where(a => !a.IsReadOnly);

        List<int> channelGroupIds = await toDeleteQuery.Select(a => a.Id).ToListAsync(cancellationToken).ConfigureAwait(false);

        List<VideoStreamDto> videoStreams = await Repository.VideoStream.GetVideoStreamsForChannelGroups(channelGroupIds, cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"Preparing to bulk delete {channelGroupIds.Count} ChannelGroups.");

        await RepositoryContext.BulkDeleteAsyncEntities(toDeleteQuery, cancellationToken: cancellationToken).ConfigureAwait(false);

        logger.LogInformation($"Successfully deleted {channelGroupIds.Count} ChannelGroups.");

        return APIResponse.Ok;
    }


    public async Task<List<ChannelGroup>> GetChannelGroupsForStreamGroup(int streamGroupId, CancellationToken cancellationToken)
    {
        List<ChannelGroup> channelGroups = await base.GetQuery().ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

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

    public async Task<APIResponse> DeleteChannelGroupsRequest(List<int> channelGroupIds)
    {
        foreach (int channelGroupId in channelGroupIds)
        {
            ChannelGroup? cg = await GetChannelGroupById(channelGroupId);
            if (cg != null)
            {
                Delete(cg);
            }
        }
        await SaveChangesAsync();
        return APIResponse.Ok;
    }

    public Task<APIResponse> DeleteChannelGroupsByNameRequest(List<string> channelGroupNames)
    {
        throw new NotImplementedException();
    }

}