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
        IQueryable<ChannelGroup> toDelete = GetQuery().Where(a => channelGroupIds.Contains(a.Id) && !a.IsReadOnly);


        await BulkDeleteAsync(toDelete);
        return APIResponse.Ok;
    }

    public async Task<APIResponse> DeleteChannelGroupsByNameRequest(List<string> channelGroupNames)
    {
        IQueryable<ChannelGroup> toDelete = GetQuery().Where(a => channelGroupNames.Contains(a.Name) && !a.IsReadOnly);
        await BulkDeleteAsync(toDelete);
        return APIResponse.Ok;
    }

}