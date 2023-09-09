using AutoMapper;
using AutoMapper.QueryableExtensions;

using EFCore.BulkExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupChannelGroupRepository(RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, ISender sender) : RepositoryBase<StreamGroupChannelGroup>(repositoryContext), IStreamGroupChannelGroupRepository
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IRepositoryWrapper _repository = repository;

    public async Task<StreamGroupDto?> SyncStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default)
    {
        // Check if the stream group exists.
        if (StreamGroupId == 0 || !RepositoryContext.StreamGroups.Any(a => a.Id == StreamGroupId))
        {
            return null;
        }

        // Fetch existing channel groups for the stream group.
        List<int> existingChannelGroupIds = await FindAll()
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

        return await _sender.Send(new GetStreamGroup(StreamGroupId), cancellationToken);
    }

    private async Task HandleAdditions(int StreamGroupId, List<int> cgsToAdd, CancellationToken cancellationToken)
    {
        if (!cgsToAdd.Any())
        {
            return;
        }

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
        List<VideoStreamDto> toAddVids = await _sender.Send(new GetVideoStreamsForChannelGroups(cgsToAdd), cancellationToken).ConfigureAwait(false);
        List<string> toAdd = toAddVids.Select(a => a.Id).Except(existingVideoStreamIds).ToList();
        List<string> toUpdate = toAddVids.Select(a => a.Id).Intersect(existingVideoStreamIds).ToList();

        if (toAdd.Any())
        {
            await _repository.StreamGroupVideoStream.AddStreamGroupVideoStreams(StreamGroupId, toAdd, true, cancellationToken).ConfigureAwait(false);
        }

        if (toUpdate.Any())
        {
            await _repository.StreamGroupVideoStream.SetStreamGroupVideoStreamsIsReadOnly(StreamGroupId, toUpdate, true, cancellationToken).ConfigureAwait(false);
        }
    }
    private async Task HandleRemovals(int StreamGroupId, List<int> cgsToRemove, CancellationToken cancellationToken)
    {
        if (!cgsToRemove.Any())
        {
            return;
        }

        // Remove channel groups from stream group.
        IQueryable<StreamGroupChannelGroup> deleteSGQ = RepositoryContext.StreamGroupChannelGroups.Where(x => x.StreamGroupId == StreamGroupId && cgsToRemove.Contains(x.ChannelGroupId));
        await RepositoryContext.BulkDeleteAsync(deleteSGQ, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Remove video streams from stream group.
        List<VideoStreamDto> toDelete = await _sender.Send(new GetVideoStreamsForChannelGroups(cgsToRemove), cancellationToken).ConfigureAwait(false);
        List<string> toRemove = toDelete.Select(a => a.Id).ToList();
        await _repository.StreamGroupVideoStream.RemoveStreamGroupVideoStreams(StreamGroupId, toRemove, cancellationToken).ConfigureAwait(false);
    }


    /// <summary>
    /// Removes the specified channel group from the specified stream group and returns a list of removed video stream IDs.
    /// </summary>
    /// <param name="StreamGroupId">The ID of the stream group.</param>
    /// <param name="ChannelGroupIds">The list of IDs of the channel groups to be removed.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A list of removed video stream IDs.</returns>
    public async Task<IEnumerable<string>> RemoveStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default)
    {
        // List to hold the IDs of removed video streams.
        List<string> removedVideoStreamIds = new();

        // Initial checks.
        if (StreamGroupId == 0 || !RepositoryContext.StreamGroups.Any(a => a.Id == StreamGroupId))
        {
            return removedVideoStreamIds;
        }

        // Get existing channel groups for the stream group.
        List<int> existingChannelGroupIds = await FindAll()
            .Where(x => x.StreamGroupId == StreamGroupId)
            .Select(x => x.ChannelGroupId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (!existingChannelGroupIds.Any())
        {
            return removedVideoStreamIds;
        }

        // Remove the stream group channel groups.
        List<StreamGroupChannelGroup> streamGroupChannelGroupsToRemove = await FindAll()
            .Where(x => x.StreamGroupId == StreamGroupId && existingChannelGroupIds.Contains(x.ChannelGroupId))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        RepositoryContext.StreamGroupChannelGroups.RemoveRange(streamGroupChannelGroupsToRemove);
        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        // Get the video streams associated with the channel groups to be removed.
        List<VideoStreamDto> vidsToRemove = await _sender.Send(new GetVideoStreamsForChannelGroups(existingChannelGroupIds), cancellationToken).ConfigureAwait(false);
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

    public async Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroups(List<int> channelGroupIds, CancellationToken cancellationToken = default)
    {
        return await FindAll()
                .Include(a => a.StreamGroup)
                .Where(x => channelGroupIds.Contains(x.ChannelGroupId))
                .Select(x => x.StreamGroup)
                .ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async Task<List<StreamGroupDto>> GetStreamGroupsFromChannelGroup(int channelGroupId, CancellationToken cancellationToken = default)
    {
        return await FindAll()
                .Include(a => a.StreamGroup)
                .Where(x => x.ChannelGroupId == channelGroupId)
                .Select(x => x.StreamGroup)
                .ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async Task<IEnumerable<ChannelGroupDto>> GetChannelGroupsFromStreamGroup(int StreamGroupId, CancellationToken cancellationToken)
    {
        return await FindAll()
                 .Include(a => a.ChannelGroup)
                 .Where(x => x.StreamGroupId == StreamGroupId)
                 .Select(x => x.ChannelGroup)
                 .ProjectTo<ChannelGroupDto>(_mapper.ConfigurationProvider)
                 .ToListAsync(cancellationToken: cancellationToken)
                 .ConfigureAwait(false);
    }
}