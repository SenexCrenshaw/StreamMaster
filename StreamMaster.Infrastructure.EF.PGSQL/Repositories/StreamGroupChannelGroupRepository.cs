using AutoMapper;
using AutoMapper.QueryableExtensions;

using EFCore.BulkExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using StreamMaster.Application.StreamGroups.Queries;
using StreamMaster.Application.VideoStreams.Queries;
using StreamMaster.Infrastructure.EF.PGSQL;

namespace StreamMaster.Infrastructure.EF.PGSQL.Repositories;

public class StreamGroupChannelGroupRepository(ILogger<StreamGroupChannelGroupRepository> intLogger, RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, ISender sender) : RepositoryBase<StreamGroupChannelGroup>(repositoryContext, intLogger), IStreamGroupChannelGroupRepository
{
    /// <summary>
    /// Synchronizes channel groups for a stream group, adding and removing them as necessary.
    /// </summary>
    /// <param name="StreamGroupId">The ID of the stream group.</param>
    /// <param name="ChannelGroupIds">The list of channel group IDs to sync.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A StreamGroupDto if the operation is successful, otherwise null.</returns>
    public async Task<StreamGroupDto?> SyncStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default)
    {
        // Check if the stream group exists.
        if (StreamGroupId == 0 || !RepositoryContext.StreamGroups.Any(a => a.Id == StreamGroupId))
        {
            return null;
        }

        // Fetch existing channel groups for the stream group.
        List<int> existingChannelGroupIds = await RepositoryContext.StreamGroupChannelGroups
            .Where(x => x.StreamGroupId == StreamGroupId)
            .Select(x => x.ChannelGroupId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        // Determine channel groups to add and remove.
        List<int> cgsToAdd = ChannelGroupIds.Except(existingChannelGroupIds).ToList();
        List<int> cgsToRemove = existingChannelGroupIds.Except(ChannelGroupIds).ToList();

        if (!cgsToAdd.Any() && !cgsToRemove.Any())
        {
            return null;
        }

        await HandleAdditions(StreamGroupId, cgsToAdd, cancellationToken);
        await HandleRemovals(StreamGroupId, cgsToRemove, cancellationToken);

        return await sender.Send(new GetStreamGroup(StreamGroupId), cancellationToken);
    }

    /// <summary>
    /// Handles the addition of channel groups and associated video streams to a stream group.
    /// </summary>
    /// <param name="StreamGroupId">The ID of the stream group.</param>
    /// <param name="cgsToAdd">The list of channel groups to add.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    public async Task HandleAdditions(int StreamGroupId, List<int> cgsToAdd, CancellationToken cancellationToken)
    {
        if (StreamGroupId <= 0 || cgsToAdd == null || !cgsToAdd.Any())
        {
            // Parameter error checks, return if StreamGroupId is invalid or cgsToAdd is null or empty
            return;
        }

        try
        {
            // Create new stream group channel groups.
            List<StreamGroupChannelGroup> streamGroupChannelGroups = cgsToAdd.Select(channelGroupId => new StreamGroupChannelGroup
            {
                StreamGroupId = StreamGroupId,
                ChannelGroupId = channelGroupId
            }).ToList();

            await RepositoryContext.StreamGroupChannelGroups.AddRangeAsync(streamGroupChannelGroups, cancellationToken).ConfigureAwait(false);
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Fetch existing video streams for the stream group.
            List<string> existingVideoStreamIds = await RepositoryContext.StreamGroupVideoStreams
                .Where(a => a.StreamGroupId == StreamGroupId)
                .Select(a => a.ChildVideoStreamId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Handle additions...
            List<VideoStreamDto> toAddVids = await sender.Send(new GetVideoStreamsForChannelGroups(cgsToAdd), cancellationToken).ConfigureAwait(false);
            List<string> toAdd = toAddVids.Select(a => a.Id).Except(existingVideoStreamIds).ToList();
            List<string> toUpdate = toAddVids.Select(a => a.Id).Intersect(existingVideoStreamIds).ToList();

            if (toAdd.Any())
            {
                await repository.StreamGroupVideoStream.AddStreamGroupVideoStreams(StreamGroupId, toAdd, true, cancellationToken).ConfigureAwait(false);
            }

            if (toUpdate.Any())
            {
                await repository.StreamGroupVideoStream.SetStreamGroupVideoStreamsIsReadOnly(StreamGroupId, toUpdate, true, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, $"An error occurred while handling additions for stream group with ID {StreamGroupId}.");
            throw; // Re-throw the exception to the caller
        }
    }

    /// <summary>
    /// Handles the removal of channel groups and associated video streams from a stream group.
    /// </summary>
    /// <param name="StreamGroupId">The ID of the stream group.</param>
    /// <param name="cgsToRemove">The list of channel groups to remove.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    public async Task HandleRemovals(int StreamGroupId, List<int> cgsToRemove, CancellationToken cancellationToken)
    {
        if (StreamGroupId <= 0 || cgsToRemove == null || !cgsToRemove.Any())
        {
            // Parameter error checks, return if StreamGroupId is invalid or cgsToRemove is null or empty
            return;
        }

        try
        {
            // Remove channel groups from the stream group.
            IQueryable<StreamGroupChannelGroup> deleteSGQ = RepositoryContext.StreamGroupChannelGroups
                .Where(x => x.StreamGroupId == StreamGroupId && cgsToRemove.Contains(x.ChannelGroupId));

            await RepositoryContext.BulkDeleteAsync(deleteSGQ, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Remove video streams from the stream group.
            List<VideoStreamDto> toDelete = await sender.Send(new GetVideoStreamsForChannelGroups(cgsToRemove), cancellationToken).ConfigureAwait(false);
            List<string> toRemove = toDelete.Select(a => a.Id).ToList();
            await repository.StreamGroupVideoStream.RemoveStreamGroupVideoStreams(StreamGroupId, toRemove, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, $"An error occurred while removing channel groups from stream group with ID {StreamGroupId}.");
            throw; // Re-throw the exception to the caller
        }
    }


    /// <summary>
    /// Removes the specified channel group from the specified stream group and returns a list of removed video stream IDs.
    /// </summary>
    /// <param name="StreamGroupId">The ID of the stream group.</param>
    /// <param name="ChannelGroupIds">The list of IDs of the channel groups to be removed.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of removed video stream IDs. If no records are found or if input is invalid, returns an empty list.</returns>
    public async Task<List<string>> RemoveStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default)
    {
        // List to hold the IDs of removed video streams.
        List<string> removedVideoStreamIds = [];

        // Initial checks.
        if (StreamGroupId <= 0 || !RepositoryContext.StreamGroups.Any(a => a.Id == StreamGroupId) || ChannelGroupIds == null || !ChannelGroupIds.Any())
        {
            return removedVideoStreamIds;
        }

        try
        {
            // Get existing channel groups for the stream group.
            List<int> existingChannelGroupIds = await RepositoryContext.StreamGroupChannelGroups
                .Where(x => x.StreamGroupId == StreamGroupId)
                .Select(x => x.ChannelGroupId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            if (!existingChannelGroupIds.Any())
            {
                return removedVideoStreamIds;
            }

            // Remove the stream group channel groups.
            List<StreamGroupChannelGroup> streamGroupChannelGroupsToRemove = await RepositoryContext.StreamGroupChannelGroups
                .Where(x => x.StreamGroupId == StreamGroupId && existingChannelGroupIds.Contains(x.ChannelGroupId))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            RepositoryContext.StreamGroupChannelGroups.RemoveRange(streamGroupChannelGroupsToRemove);
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Get the video streams associated with the channel groups to be removed.
            List<VideoStreamDto> vidsToRemove = await sender.Send(new GetVideoStreamsForChannelGroups(existingChannelGroupIds), cancellationToken).ConfigureAwait(false);
            List<string> vidIdsToRemove = vidsToRemove.Select(a => a.Id).ToList();

            // Get the video streams from the stream group that need to be removed.
            List<StreamGroupVideoStream> streamGroupVideoStreamsToRemove = await RepositoryContext.StreamGroupVideoStreams
                .Where(a => a.StreamGroupId == StreamGroupId && vidIdsToRemove.Contains(a.ChildVideoStreamId))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            // Add the IDs of the video streams to be removed to the return list.
            removedVideoStreamIds.AddRange(streamGroupVideoStreamsToRemove.Select(a => a.ChildVideoStreamId));

            // Remove the associated video streams from the stream group.
            RepositoryContext.StreamGroupVideoStreams.RemoveRange(streamGroupVideoStreamsToRemove);
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return removedVideoStreamIds;
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, $"An error occurred while removing channel groups from stream group with ID {StreamGroupId}.");
            throw; // Re-throw the exception to the caller
        }
    }

    /// <summary>
    /// Gets a list of stream groups associated with a list of channel group IDs.
    /// </summary>
    /// <param name="channelGroupIds">The list of channel group IDs.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of StreamGroupDto objects. If no records are found or if input is null, returns an empty list.</returns>
    public async Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroups(List<int> channelGroupIds, CancellationToken cancellationToken = default)
    {
        if (channelGroupIds == null || !channelGroupIds.Any())
        {
            // Return an empty list if the input is null or empty
            return [];
        }

        try
        {
            // Query the database
            List<StreamGroupDto> streamGroups = await RepositoryContext.StreamGroupChannelGroups
                .Include(a => a.StreamGroup)
                .Where(x => channelGroupIds.Contains(x.ChannelGroupId))
                .Select(x => x.StreamGroup)
                .ProjectTo<StreamGroupDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return streamGroups;
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, "An error occurred while getting stream groups from channel groups.");
            throw; // Re-throw the exception to the caller
        }
    }

    /// <summary>
    /// Gets a list of channel groups associated with a specific stream group ID.
    /// </summary>
    /// <param name="StreamGroupId">The ID of the stream group.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of ChannelGroupDto objects. If no records are found, returns an empty list.</returns>
    public async Task<List<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(int StreamGroupId, CancellationToken cancellationToken)
    {
        if (StreamGroupId <= 0)
        {
            throw new ArgumentException("StreamGroupId must be greater than zero.", nameof(StreamGroupId));
        }

        try
        {
            // Query the database
            List<ChannelGroupDto> channelGroups = await RepositoryContext.StreamGroupChannelGroups
                .Include(a => a.ChannelGroup)
                .Where(x => x.StreamGroupId == StreamGroupId)
                .Select(x => x.ChannelGroup)
                .ProjectTo<ChannelGroupDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return channelGroups;
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, $"An error occurred while getting channel groups for stream group with ID {StreamGroupId}.");
            throw; // Re-throw the exception to the caller
        }
    }



    /// <summary>
    /// Gets a list of stream groups associated with a specific channel group ID.
    /// </summary>
    /// <param name="channelGroupId">The ID of the channel group.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A list of StreamGroupDto objects. If no records are found or if input is invalid, returns an empty list.</returns>
    public async Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroup(int channelGroupId, CancellationToken cancellationToken = default)
    {
        if (channelGroupId <= 0)
        {
            // Return an empty list if the input is invalid
            return [];
        }

        try
        {
            // Query the database
            List<StreamGroupDto> streamGroups = await RepositoryContext.StreamGroupChannelGroups
                .Include(a => a.StreamGroup)
                .Where(x => x.ChannelGroupId == channelGroupId)
                .Select(x => x.StreamGroup)
                .ProjectTo<StreamGroupDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return streamGroups;
        }
        catch (Exception ex)
        {
            // Log the exception
            logger.LogError(ex, $"An error occurred while getting stream groups for channel group with ID {channelGroupId}.");
            throw; // Re-throw the exception to the caller
        }
    }
}