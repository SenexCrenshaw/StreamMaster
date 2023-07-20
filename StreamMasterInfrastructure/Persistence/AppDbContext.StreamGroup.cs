using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMasterApplication.StreamGroups;
using StreamMasterApplication.StreamGroups.Commands;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Entities;

using System.Text.RegularExpressions;

namespace StreamMasterInfrastructure.Persistence;

public partial class AppDbContext : IStreamGroupDB
{
    public DbSet<StreamGroupChannelGroup> StreamGroupChannelGroups { get; set; }
    public DbSet<StreamGroup> StreamGroups { get; set; }
    public DbSet<StreamGroupVideoStream> StreamGroupVideoStreams { get; set; }

    public async Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken)
    {
        // Check if combination already exists
        bool alreadyExists = await StreamGroupChannelGroups
            .AnyAsync(sgcg => sgcg.StreamGroupId == streamGroupId && sgcg.ChannelGroupId == channelGroupId, cancellationToken);

        // If combination exists, return false
        if (alreadyExists)
        {
            return false;
        }

        // If not, create new StreamGroupChannelGroup entity
        var streamGroupChannelGroup = new StreamGroupChannelGroup
        {
            StreamGroupId = streamGroupId,
            ChannelGroupId = channelGroupId
        };

        // Add new entity to DbSet
        await StreamGroupChannelGroups.AddAsync(streamGroupChannelGroup, cancellationToken);

        // Save changes in database
        await SaveChangesAsync(cancellationToken);

        // Return true indicating successful addition
        return true;
    }

    public async Task AddOrUpdatVideoStreamToStreamGroupAsync(int streamgroupId, int childId, bool isReadOnly, CancellationToken cancellationToken)
    {
        var streamGroupVideoStream = await StreamGroupVideoStreams
            .FirstOrDefaultAsync(sgcg => sgcg.StreamGroupId == streamgroupId && sgcg.ChildVideoStreamId == childId, cancellationToken).ConfigureAwait(false);

        if (streamGroupVideoStream == null)
        {
            streamGroupVideoStream = new StreamGroupVideoStream
            {
                StreamGroupId = streamgroupId,
                ChildVideoStreamId = childId,
                IsReadOnly = isReadOnly
            };

            await StreamGroupVideoStreams.AddAsync(streamGroupVideoStream, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            streamGroupVideoStream.IsReadOnly = isReadOnly;
        }

        await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken)
    {
        var streamGroup = await StreamGroups.FindAsync(new object[] { streamGroupId }, cancellationToken).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return false;
        }

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        var cgs = await StreamGroupChannelGroups
            .Where(vsl => vsl.StreamGroupId == streamGroupId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        StreamGroupChannelGroups.RemoveRange(cgs);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        var vss = await StreamGroupVideoStreams
            .Where(vsl => vsl.StreamGroupId == streamGroupId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        StreamGroupVideoStreams.RemoveRange(vss);

        // Remove the VideoStream
        StreamGroups.Remove(streamGroup);

        // Save changes
        try
        {
            await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<StreamGroup>> GetAllStreamGroupsWithRelatedEntitiesAsync(CancellationToken cancellationToken)
    {
        return await StreamGroups
            .Include(sg => sg.ChannelGroups)
                .ThenInclude(sgcg => sgcg.ChannelGroup)
            .Include(sg => sg.ChildVideoStreams)
                .ThenInclude(sgvs => sgvs.ChildVideoStream)
            .ToListAsync(cancellationToken);
    }

    public async Task<StreamGroupDto?> GetStreamGroupDto(int streamGroupId, string Url, CancellationToken cancellationToken = default)
    {
        if (streamGroupId == 0) return new StreamGroupDto { Id = 0, Name = "All" };

        StreamGroup? streamGroup = await GetStreamGroupWithRelatedEntitiesByIdAsync(streamGroupId, cancellationToken);

        if (streamGroup == null)
            return null;

        var ret = _mapper.Map<StreamGroupDto>(streamGroup);

        var existingIds = streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStreamId).ToList();

        foreach (var channegroup in streamGroup.ChannelGroups)
        {
            var streams = VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && a.User_Tvg_group == channegroup.ChannelGroup.Name)
                .AsNoTracking()
                .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider)
                .ToList();
            foreach (var stream in streams)
            {
                stream.IsReadOnly = true;
            }
            ret.ChildVideoStreams.AddRange(streams);

            var cg = await ChannelGroups.FirstOrDefaultAsync(a => a.Id == channegroup.ChannelGroupId, cancellationToken).ConfigureAwait(false);

            if (cg is not null && !string.IsNullOrEmpty(cg.RegexMatch))
            {
                var regexStreams = await GetVideoStreamsByNamePatternAsync(cg.RegexMatch, cancellationToken).ConfigureAwait(false);
                foreach (var stream in regexStreams)
                {
                    if (!existingIds.Contains(stream.Id))
                    {
                        stream.IsReadOnly = true;
                        ret.ChildVideoStreams.Add(_mapper.Map<VideoStreamDto>(stream));
                        existingIds.Add(stream.Id);
                    }
                }
            }
        }

        var relationShips = StreamGroupVideoStreams.Where(a => a.StreamGroupId == streamGroup.Id).ToList();

        foreach (var stream in ret.ChildVideoStreams)
        {
            var r = relationShips.FirstOrDefault(a => a.ChildVideoStreamId == stream.Id);
            if (r != null)
            {
                stream.IsReadOnly = r.IsReadOnly;
            }
        }
        var encodedStreamGroupNumber = ret.StreamGroupNumber.EncodeValue128(_setting.ServerKey);
        ret.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
        ret.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
        ret.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";

        return ret;
    }

    public async Task<List<StreamGroupDto>> GetStreamGroupDtos(string Url, CancellationToken cancellationToken = default)
    {
        var ret = new List<StreamGroupDto>();

        foreach (var streamGroupId in StreamGroups.Select(a => a.Id))
        {
            var streamGroup = await GetStreamGroupDto(streamGroupId, Url, cancellationToken);
            if (streamGroup == null)
                continue;
            ret.Add(streamGroup);
        }

        return ret;
    }

    public async Task<StreamGroup> GetStreamGroupWithRelatedEntitiesByIdAsync(int streamGroupId, CancellationToken cancellationToken)
    {
        return await StreamGroups
            .Include(sg => sg.ChannelGroups)
                .ThenInclude(sgcg => sgcg.ChannelGroup)
            .Include(sg => sg.ChildVideoStreams)
                .ThenInclude(sgvs => sgvs.ChildVideoStream)
            .SingleOrDefaultAsync(sg => sg.Id == streamGroupId, cancellationToken);
    }

    public async Task<StreamGroupDto> GetStreamGroupWithRelatedEntitiesByStreamGroupNumberAsync(int streamGroupNumber, CancellationToken cancellationToken)
    {
        return await StreamGroups
            .Include(sg => sg.ChannelGroups)
                .ThenInclude(sgcg => sgcg.ChannelGroup)
            .Include(sg => sg.ChildVideoStreams)
                .ThenInclude(sgvs => sgvs.ChildVideoStream)
                .ProjectTo<StreamGroupDto>(_mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(sg => sg.StreamGroupNumber == streamGroupNumber, cancellationToken);
    }

    public async Task<List<int>> GetVideoStreamIdsByStreamGroupAsync(int streamGroupId, CancellationToken cancellationToken)
    {
        // Fetch the stream group with associated channel groups
        var streamGroup = await StreamGroups
            .Include(sg => sg.ChannelGroups)
            .ThenInclude(sgcg => sgcg.ChannelGroup)
            .SingleAsync(sg => sg.Id == streamGroupId, cancellationToken);

        // Compile all regexes
        var regexes = streamGroup.ChannelGroups
            .Where(a => !string.IsNullOrEmpty(a.ChannelGroup.RegexMatch))
            .Select(cg => new Regex(cg.ChannelGroup.RegexMatch, RegexOptions.ECMAScript | RegexOptions.IgnoreCase))
            .ToList();

        // If no regexes exist, return an empty list
        if (!regexes.Any())
        {
            return new List<int>();
        }

        // Fetch all video streams
        var allVideoStreams = await VideoStreams.AsNoTracking().ToListAsync(cancellationToken);

        // Filter the video streams by matching names with regexes
        var matchingVideoStreamIds = allVideoStreams
            .Where(vs => regexes.Any(regex => regex.IsMatch(vs.User_Tvg_name)))
            .Select(vs => vs.Id)
            .ToList();

        return matchingVideoStreamIds;
    }

    public async Task<List<int>> GetVideoStreamIdsByUserGroupMatchAsync(int streamGroupId, CancellationToken cancellationToken)
    {
        // Fetch the stream group with associated channel groups
        var streamGroup = await StreamGroups
            .Include(sg => sg.ChannelGroups)
            .ThenInclude(sgcg => sgcg.ChannelGroup)
            .SingleOrDefaultAsync(sg => sg.Id == streamGroupId, cancellationToken);

        // If the stream group doesn't exist, return an empty list
        if (streamGroup == null)
        {
            return new List<int>();
        }

        // Fetch all channel group names
        var channelGroupNames = streamGroup.ChannelGroups.Select(cg => cg.ChannelGroup.Name).ToList();

        // Fetch video stream IDs that match the user group
        var matchingVideoStreamIds = await VideoStreams
            .Where(vs => channelGroupNames.Contains(vs.User_Tvg_group))
            .Select(vs => vs.Id)
            .ToListAsync(cancellationToken);

        return matchingVideoStreamIds;
    }

    public async Task<List<VideoStream>> GetVideoStreamsByNamePatternAsync(string pattern, CancellationToken cancellationToken)
    {
        var regex = new Regex(pattern, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
        var allVideoStreams = await VideoStreams.AsNoTracking().ToListAsync(cancellationToken);

        return allVideoStreams
            .Where(vs => regex.IsMatch(vs.User_Tvg_name))
            .ToList();
    }

    public async Task<bool> RemoveChildVideoStreamFromStreamGroupAsync(int streamGroupId, int videoStreamId, CancellationToken cancellationToken)
    {
        // Find the relationship in the DbSet
        var streamGroupVideoStream = await StreamGroupVideoStreams
            .SingleOrDefaultAsync(sgvs => sgvs.StreamGroupId == streamGroupId && sgvs.ChildVideoStreamId == videoStreamId, cancellationToken);

        // If not found, return false
        if (streamGroupVideoStream == null)
        {
            return false;
        }

        // If found, remove from DbSet
        StreamGroupVideoStreams.Remove(streamGroupVideoStream);

        // Save changes in database
        await SaveChangesAsync(cancellationToken);

        // Return true indicating successful removal
        return true;
    }

    public async Task<int> RemoveChildVideoStreamsFromStreamGroupAsync(int streamGroupId, List<int> videoStreamIds, CancellationToken cancellationToken)
    {
        // Find the relationships in the DbSet
        var streamGroupVideoStreams = await StreamGroupVideoStreams
            .Where(sgvs => sgvs.StreamGroupId == streamGroupId && videoStreamIds.Contains(sgvs.ChildVideoStreamId))
            .ToListAsync(cancellationToken);

        // If none found, return 0
        if (!streamGroupVideoStreams.Any())
        {
            return 0;
        }

        // If found, remove from DbSet
        StreamGroupVideoStreams.RemoveRange(streamGroupVideoStreams);

        // Save changes in database
        await SaveChangesAsync(cancellationToken);

        // Return count of removed entries
        return streamGroupVideoStreams.Count;
    }

    public async Task SynchronizeChannelGroupsInStreamGroupAsync(int streamGroupId, List<string> validChannelGroupNames, CancellationToken cancellationToken)
    {
        var streamGroup = await StreamGroups.Include(sg => sg.ChannelGroups)
            .ThenInclude(cg => cg.ChannelGroup)
            .FirstOrDefaultAsync(sg => sg.Id == streamGroupId, cancellationToken);

        if (streamGroup == null)
        {
            throw new Exception("StreamGroup not found.");
        }

        // Remove ChannelGroups not in validChannelGroupNames
        var channelGroupsToRemove = streamGroup.ChannelGroups
            .Where(cg => !validChannelGroupNames.Contains(cg.ChannelGroup.Name))
            .ToList();

        foreach (var channelGroupToRemove in channelGroupsToRemove)
        {
            streamGroup.ChannelGroups.Remove(channelGroupToRemove);
        }

        // Get list of existing ChannelGroup names to avoid adding duplicates
        var existingChannelGroupNames = streamGroup.ChannelGroups.Select(cg => cg.ChannelGroup.Name).ToList();

        // Add ChannelGroups that are not already in the StreamGroup
        foreach (var name in validChannelGroupNames)
        {
            if (!existingChannelGroupNames.Contains(name))
            {
                var channelGroupToAdd = await ChannelGroups.FirstOrDefaultAsync(cg => cg.Name == name, cancellationToken);
                if (channelGroupToAdd != null)
                {
                    streamGroup.ChannelGroups.Add(new StreamGroupChannelGroup { ChannelGroupId = channelGroupToAdd.Id, StreamGroupId = streamGroupId });
                }
            }
        }

        await SaveChangesAsync(cancellationToken);
    }

    public async Task<(int added, int removed)> SynchronizeChildVideoStreamsInStreamGroupAsync(int streamGroupId, List<VideoStreamIsReadOnly> validVideoStreams, CancellationToken cancellationToken)
    {
        // Find the relationships in the DbSet
        var streamGroupVideoStreams = await StreamGroupVideoStreams
            .Where(sgvs => sgvs.StreamGroupId == streamGroupId)
            .ToListAsync(cancellationToken);

        var videoIds = validVideoStreams.Select(a => a.VideoStreamId).ToList();

        // Determine which VideoStreams to remove
        var videoStreamsToRemove = streamGroupVideoStreams
            .Where(sgvs => !videoIds.Contains(sgvs.ChildVideoStreamId))
            .ToList();

        // Remove VideoStreams from DbSet
        StreamGroupVideoStreams.RemoveRange(videoStreamsToRemove);

        // Determine which VideoStreams to add
        var currentVideoStreamIds = streamGroupVideoStreams
            .Select(sgvs => sgvs.ChildVideoStreamId)
            .Except(videoStreamsToRemove.Select(sgvs => sgvs.ChildVideoStreamId))
            .ToList();

        var videoStreamIdsToAdd = videoIds.Except(currentVideoStreamIds).ToList();
        var videoStreamsToAdd = videoStreamIdsToAdd.Select(videoStreamId => new StreamGroupVideoStream
        {
            StreamGroupId = streamGroupId,
            ChildVideoStreamId = videoStreamId,
            IsReadOnly = validVideoStreams.Single(a => a.VideoStreamId == videoStreamId).IsReadOnly
        }).ToList();

        // Add VideoStreams to DbSet
        await StreamGroupVideoStreams.AddRangeAsync(videoStreamsToAdd, cancellationToken);

        // Save changes in database
        await SaveChangesAsync(cancellationToken);

        // Return count of added and removed entries
        return (added: videoStreamsToAdd.Count, removed: videoStreamsToRemove.Count);
    }

    public async Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, string Url, CancellationToken cancellationToken)
    {
        try
        {
            var streamGroup = await GetStreamGroupDto(request.StreamGroupId, Url, cancellationToken).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                streamGroup.Name = request.Name;
            }

            if (request.StreamGroupNumber != null)
            {
                if (!await StreamGroups.AnyAsync(a => a.StreamGroupNumber == (int)request.StreamGroupNumber, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    streamGroup.StreamGroupNumber = (int)request.StreamGroupNumber;
                }
            }

            var channelIds = new List<int>();

            if (request.ChannelGroupNames != null)
            {
                await SynchronizeChannelGroupsInStreamGroupAsync(streamGroup.Id, request.ChannelGroupNames, cancellationToken);
            }

            if (streamGroup.ChannelGroups.Any())
            {
                var fromRegex = await GetVideoStreamIdsByStreamGroupAsync(streamGroup.Id, cancellationToken);
                channelIds = await GetVideoStreamIdsByUserGroupMatchAsync(streamGroup.Id, cancellationToken);
                channelIds = channelIds.Concat(fromRegex).ToList();

                if (streamGroup.ChildVideoStreams.Any(a => channelIds.Contains(a.Id)))
                {
                    var toRemove = streamGroup.ChildVideoStreams.Where(a => channelIds.Contains(a.Id)).ToList();
                    await RemoveChildVideoStreamsFromStreamGroupAsync(streamGroup.Id, toRemove.Select(a => a.Id).ToList(), cancellationToken).ConfigureAwait(false);
                }
            }

            if (request.VideoStreams != null)
            {
                var toDo = request.VideoStreams.Where(a => !channelIds.Contains(a.VideoStreamId)).ToList();
                await SynchronizeChildVideoStreamsInStreamGroupAsync(streamGroup.Id, toDo, cancellationToken).ConfigureAwait(false);
            }

            var ret = await GetStreamGroupDto(streamGroup.Id, Url, cancellationToken);
            return ret;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
