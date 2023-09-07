using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Repository;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupChannelGroupRepository(RepositoryContext repositoryContext, IRepositoryWrapper repository, IMapper mapper, ISender sender) : RepositoryBase<StreamGroupChannelGroup>(repositoryContext), IStreamGroupChannelGroupRepository
{
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IRepositoryWrapper _repository = repository;

    public async Task<int> SyncStreamGroupChannelGroups(int StreamGroupId, List<int> ChannelGroupIds, CancellationToken cancellationToken = default)
    {
        // Initial checks.
        if (StreamGroupId == 0 || !RepositoryContext.StreamGroups.Any(a => a.Id == StreamGroupId))
        {
            return 0;
        }

        // Get existing channel groups for the stream group.
        List<int> existingChannelGroupIds = await FindAll()
            .Where(x => x.StreamGroupId == StreamGroupId)
            .Select(x => x.ChannelGroupId)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        // Determine new channel groups to add.
        List<int> cgsToAdd = ChannelGroupIds.Except(existingChannelGroupIds).ToList();
        List<int> cgsToRemove = existingChannelGroupIds.Except(ChannelGroupIds).ToList();

        if (!cgsToAdd.Any() && !cgsToRemove.Any())
        {
            return 0;
        }

        if (cgsToAdd.Any())
        {
            // Create new stream group channel groups.
            List<StreamGroupChannelGroup> streamGroupChannelGroups = cgsToAdd.Select(channelGroupId => new StreamGroupChannelGroup
            {
                StreamGroupId = StreamGroupId,
                ChannelGroupId = channelGroupId
            }).ToList();
            await RepositoryContext.StreamGroupChannelGroups.AddRangeAsync(streamGroupChannelGroups, cancellationToken).ConfigureAwait(false);
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        if (cgsToRemove.Any())
        {
            await RepositoryContext.StreamGroupChannelGroups.Where(x => x.StreamGroupId == StreamGroupId && cgsToRemove.Contains(x.ChannelGroupId)).ExecuteDeleteAsync(cancellationToken: cancellationToken);
        }


        // Get existing video streams for the stream group.
        List<VideoStreamDto> existingVideoStreams = await RepositoryContext.StreamGroupVideoStreams
            .Where(a => a.StreamGroupId == StreamGroupId)
            .Select(a => a.ChildVideoStream)
            .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        List<string> existingIds = existingVideoStreams.Select(a => a.Id).ToList();
        List<int> cgs = cgsToAdd.Concat(cgsToRemove).ToList();
        // Get video streams associated with the channel groups.
        List<VideoStreamDto> vids = await _sender.Send(new GetVideoStreamsForChannelGroups(cgs), cancellationToken).ConfigureAwait(false);
        List<string> vidIds = vids.Select(a => a.Id).ToList();

        // Determine video streams to remove and add.
        List<string> toRemove = existingIds.Intersect(vidIds).ToList();
        List<string> toAdd = vidIds.Except(existingIds).ToList();

        if (toRemove.Any())
        {
            await _repository.StreamGroupVideoStream.RemoveStreamGroupVideoStreams(StreamGroupId, toRemove, cancellationToken).ConfigureAwait(false);
        }
        if (toAdd.Any())
        {
            await _repository.StreamGroupVideoStream.AddStreamGroupVideoStreams(StreamGroupId, toAdd, true, cancellationToken).ConfigureAwait(false);
        }

        return toRemove.Count + toAdd.Count;
    }


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