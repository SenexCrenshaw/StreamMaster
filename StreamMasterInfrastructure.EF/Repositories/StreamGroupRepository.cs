using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupRepository : RepositoryBase<StreamGroup>, IStreamGroupRepository
{
    private readonly ISortHelper<StreamGroup> _StreamGroupSortHelper;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public StreamGroupRepository(RepositoryContext repositoryContext, ISortHelper<StreamGroup> StreamGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender) : base(repositoryContext)
    {
        _sender = sender;
        _mapper = mapper;
        _StreamGroupSortHelper = StreamGroupSortHelper;
    }

    public async Task<StreamGroupDto?> GetStreamGroupDtoByStreamGroupNumber(int streamGroupNumber, string Url, CancellationToken cancellationToken = default)
    {
        StreamGroup? sg = await GetStreamGroupByStreamGroupNumberAsync(streamGroupNumber).ConfigureAwait(false);
        if (sg is not null)
        {
            return await GetStreamGroupDto(sg?.Id ?? 0, Url, cancellationToken).ConfigureAwait(false);
        }
        return null;
    }

    public async Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken)
    {
        await RepositoryContext.VideoStreams
              .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
              .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, newGroupName), cancellationToken: cancellationToken)
              .ConfigureAwait(false);
    }

    public async Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken)
    {
        // Check if combination already exists
        bool alreadyExists = await RepositoryContext.StreamGroupChannelGroups
            .AnyAsync(sgcg => sgcg.StreamGroupId == streamGroupId && sgcg.ChannelGroupId == channelGroupId, cancellationToken);

        // If combination exists, return false
        if (alreadyExists)
        {
            return false;
        }

        // If not, create new StreamGroupChannelGroup entity
        StreamGroupChannelGroup streamGroupChannelGroup = new()
        {
            StreamGroupId = streamGroupId,
            ChannelGroupId = channelGroupId
        };

        // Add new entity to DbSet
        await RepositoryContext.StreamGroupChannelGroups.AddAsync(streamGroupChannelGroup, cancellationToken);

        // Save changes in database
        await RepositoryContext.SaveChangesAsync(cancellationToken);

        // Return true indicating successful addition
        return true;
    }

    public async Task<IPagedList<StreamGroup>> GetStreamGroupsAsync(StreamGroupParameters StreamGroupParameters)
    {
        IQueryable<StreamGroup> StreamGroups = FindAll();

        IQueryable<StreamGroup> sorderStreamGroups = _StreamGroupSortHelper.ApplySort(StreamGroups, StreamGroupParameters.OrderBy);

        return await sorderStreamGroups.ToPagedListAsync(StreamGroupParameters.PageNumber, StreamGroupParameters.PageSize).ConfigureAwait(false);
    }

    public async Task<StreamGroup?> GetStreamGroupByIdAsync(int id)
    {
        return await FindByCondition(StreamGroup => StreamGroup.Id == id).FirstOrDefaultAsync();
    }

    public async Task<StreamGroup?> GetStreamGroupByStreamGroupNumberAsync(int streamGroupNumber)
    {
        return await FindByCondition(StreamGroup => StreamGroup.StreamGroupNumber == streamGroupNumber).FirstOrDefaultAsync();
    }

    private async Task<StreamGroup?> GetStreamGroupWithRelatedEntitiesByIdAsync(int streamGroupId, CancellationToken cancellationToken)
    {
        return await RepositoryContext.StreamGroups
            .Include(sg => sg.ChannelGroups)
                .ThenInclude(sgcg => sgcg.ChannelGroup)
            .Include(sg => sg.ChildVideoStreams)
                .ThenInclude(sgvs => sgvs.ChildVideoStream)
            .SingleOrDefaultAsync(sg => sg.Id == streamGroupId, cancellationToken);
    }

    public async Task<List<VideoStreamIsReadOnly>> GetStreamGroupVideoStreamIds(int id, CancellationToken cancellationToken = default)
    {
        if (id == 0)
        {
            return new();
        }

        List<VideoStreamIsReadOnly> ret = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id)
            .AsNoTracking()
            .Select(a => a.ChildVideoStreamId)
            .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a, IsReadOnly = false }).ToListAsync();

        List<string> existingIds = ret.Select(a => a.VideoStreamId).ToList();

        List<string> cgNames = await RepositoryContext.StreamGroupChannelGroups.AsNoTracking()
            .Where(a => a.StreamGroupId == id)
            .Select(a => a.ChannelGroup.Name)
            .ToListAsync();

        List<VideoStreamIsReadOnly> streams = await RepositoryContext.VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && cgNames.Contains(a.User_Tvg_group))
                .Select(a => a.Id)
                .AsNoTracking()
                .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a, IsReadOnly = true }).ToListAsync();

        ret.AddRange(streams);

        return ret;
    }

    public async Task<StreamGroupDto?> GetStreamGroupDto(int id, string Url, CancellationToken cancellationToken = default)
    {
        if (id == 0) return new StreamGroupDto { Id = 0, Name = "All" };

        StreamGroup? streamGroup = await GetStreamGroupWithRelatedEntitiesByIdAsync(id, cancellationToken);

        if (streamGroup == null)
            return null;

        StreamGroupDto ret = _mapper.Map<StreamGroupDto>(streamGroup);
        Setting _setting = FileUtil.GetSetting();
        string encodedStreamGroupNumber = ret.StreamGroupNumber.EncodeValue128(_setting.ServerKey);
        ret.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
        ret.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
        ret.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
        return ret;
    }

    public IQueryable<StreamGroup> GetAllStreamGroups()
    {
        return FindAll().OrderBy(p => p.Id);
    }

    public IQueryable<StreamGroup> GetAllStreamGroupsWithChannelGroups()
    {
        return FindAll().Include(sg => sg.ChannelGroups)
            .ThenInclude(sgcg => sgcg.ChannelGroup).OrderBy(p => p.Name);
    }

    public async Task<List<StreamGroupDto>> GetStreamGroupDtos(string Url, CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> ret = new();

        foreach (int streamGroupId in GetAllStreamGroups().Select(a => a.Id))
        {
            StreamGroupDto? streamGroup = await GetStreamGroupDto(streamGroupId, Url, cancellationToken);
            if (streamGroup == null)
                continue;
            ret.Add(streamGroup);
        }

        return ret;
    }

    public void CreateStreamGroup(StreamGroup StreamGroup)
    {
        Create(StreamGroup);
    }

    public async Task<bool> DeleteStreamGroupsync(int streamGroupId, CancellationToken cancellationToken)
    {
        StreamGroup? streamGroup = await GetStreamGroupByIdAsync(streamGroupId).ConfigureAwait(false);
        if (streamGroup == null)
        {
            return false;
        }

        // Remove associated VideoStreamLinks where the VideoStream is a parent
        List<StreamGroupChannelGroup> cgs = await RepositoryContext.StreamGroupChannelGroups
            .Where(vsl => vsl.StreamGroupId == streamGroupId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        RepositoryContext.StreamGroupChannelGroups.RemoveRange(cgs);

        // Remove associated VideoStreamLinks where the VideoStream is a child
        List<StreamGroupVideoStream> vss = await RepositoryContext.StreamGroupVideoStreams
            .Where(vsl => vsl.StreamGroupId == streamGroupId)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
        RepositoryContext.StreamGroupVideoStreams.RemoveRange(vss);

        // Remove the VideoStream
        Delete(streamGroup);

        // Save changes
        try
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task AddVideoStreamToStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroup? streamGroup = await GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return;
            }

            StreamGroupDto? streamGroupDto = await GetStreamGroupDto(StreamGroupId, "", cancellationToken).ConfigureAwait(false);

            if (streamGroupDto == null)
            {
                return;
            }

            var sgVs = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == StreamGroupId).AsNoTracking()
                .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a.ChildVideoStreamId, IsReadOnly = a.IsReadOnly, Rank = a.Rank }).ToListAsync(cancellationToken: cancellationToken);
            sgVs.Add(new VideoStreamIsReadOnly { VideoStreamId = VideoStreamId, IsReadOnly = false, Rank = sgVs.Count });

            await Syncthing(StreamGroupId, "", null, sgVs, cancellationToken);
        }
        catch (Exception)
        {
        }
    }

    public async Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, string Url, CancellationToken cancellationToken)
    {
        try
        {
            StreamGroup? streamGroup = await GetStreamGroupByIdAsync(request.StreamGroupId).ConfigureAwait(false);

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
                if (!await RepositoryContext.StreamGroups.AnyAsync(a => a.StreamGroupNumber == (int)request.StreamGroupNumber, cancellationToken: cancellationToken).ConfigureAwait(false))
                {
                    streamGroup.StreamGroupNumber = (int)request.StreamGroupNumber;
                }
            }

            UpdateStreamGroup(streamGroup);

            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            await Syncthing(streamGroup.Id, Url, request.ChannelGroupNames, request.VideoStreams, cancellationToken);

            StreamGroupDto? ret = await GetStreamGroupDto(streamGroup.Id, Url, cancellationToken);
            return ret;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task<StreamGroupDto?> Syncthing(int streamGroupId, string Url, List<string>? ChannelGroupNames, List<VideoStreamIsReadOnly>? VideoStreams, CancellationToken cancellationToken = default)
    {
        try
        {
            StreamGroupDto? streamGroupDto = await GetStreamGroupDto(streamGroupId, Url, cancellationToken).ConfigureAwait(false);

            if (streamGroupDto == null)
            {
                return null;
            }

            await SynchronizeStreamGroupChannelsAndVideoStreams(streamGroupDto, ChannelGroupNames, VideoStreams, cancellationToken);

            StreamGroupDto? ret = await GetStreamGroupDto(streamGroupId, Url, cancellationToken).ConfigureAwait(false);
            return ret;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task SynchronizeStreamGroupChannelsAndVideoStreams(
            StreamGroupDto streamGroup,
            List<string>? ChannelGroupNames,
            List<VideoStreamIsReadOnly>? VideoStreams,
            CancellationToken cancellationToken
        )
    {
        List<string> channelIds = new();

        if (ChannelGroupNames != null)
        {
            await SynchronizeChannelGroupsInStreamGroupAsync(streamGroup.Id, ChannelGroupNames, cancellationToken);
        }

        if (streamGroup.ChannelGroups.Any())
        {
            channelIds = await _sender.Send(new GetVideoStreamIdsByStreamGroupQuery(streamGroup.Id), cancellationToken).ConfigureAwait(false);

            if (streamGroup.ChildVideoStreams.Any(a => channelIds.Contains(a.Id)))
            {
                List<VideoStreamDto> toRemove = streamGroup.ChildVideoStreams.Where(a => channelIds.Contains(a.Id)).ToList();
                await RemoveChildVideoStreamsFromStreamGroupAsync(streamGroup.Id, toRemove.Select(a => a.Id).ToList(), cancellationToken).ConfigureAwait(false);
            }
        }

        if (VideoStreams != null)
        {
            List<VideoStreamIsReadOnly> toDo = VideoStreams.Where(a => !channelIds.Contains(a.VideoStreamId)).ToList();
            await SynchronizeChildVideoStreamsInStreamGroupAsync(streamGroup.Id, toDo, cancellationToken).ConfigureAwait(false);
        }
        await SynchronizeChannelGroupsInStreamGroupAsync(streamGroup.Id, ChannelGroupNames, cancellationToken);
    }

    private async Task SynchronizeChildVideoStreamsInStreamGroupAsync(int streamGroupId, List<VideoStreamIsReadOnly> validVideoStreams, CancellationToken cancellationToken)
    {
        //// Find the relationships in the DbSet
        List<StreamGroupVideoStream> streamGroupVideoStreams = await RepositoryContext.StreamGroupVideoStreams
            .Where(sgvs => sgvs.StreamGroupId == streamGroupId)
            .ToListAsync(cancellationToken);

        List<string> videoIds = validVideoStreams.Select(a => a.VideoStreamId).ToList();

        RepositoryContext.StreamGroupVideoStreams.RemoveRange(streamGroupVideoStreams);
        await RepositoryContext.SaveChangesAsync(cancellationToken);

        var toAdd = new List<StreamGroupVideoStream>();
        for (int i = 0; i < validVideoStreams.Count; i++)
        {
            VideoStreamIsReadOnly? item = validVideoStreams[i];
            var stream = RepositoryContext.VideoStreams.FirstOrDefault(a => a.Id == item.VideoStreamId);
            if (stream != null)
            {
                toAdd.Add(new StreamGroupVideoStream
                {
                    StreamGroupId = streamGroupId,
                    ChildVideoStreamId = stream.Id,
                    IsReadOnly = validVideoStreams.Single(a => a.VideoStreamId == stream.Id).IsReadOnly,
                    Rank = item.Rank
                });
            }
        }

        // Add VideoStreams to DbSet
        await RepositoryContext.StreamGroupVideoStreams.AddRangeAsync(toAdd, cancellationToken);

        // Save changes in database
        await RepositoryContext.SaveChangesAsync(cancellationToken);

        // Return count of added and removed entries
        return; // (added: videoStreamsToAdd.Count, removed: videoStreamsToRemove.Count);
    }

    private async Task<int> RemoveChildVideoStreamsFromStreamGroupAsync(int streamGroupId, List<string> videoStreamIds, CancellationToken cancellationToken)
    {
        // Find the relationships in the DbSet
        List<StreamGroupVideoStream> streamGroupVideoStreams = await RepositoryContext.StreamGroupVideoStreams
            .Where(sgvs => sgvs.StreamGroupId == streamGroupId && videoStreamIds.Contains(sgvs.ChildVideoStreamId))
            .ToListAsync(cancellationToken);

        // If none found, return 0
        if (!streamGroupVideoStreams.Any())
        {
            return 0;
        }

        // If found, remove from DbSet
        RepositoryContext.StreamGroupVideoStreams.RemoveRange(streamGroupVideoStreams);

        // Save changes in database
        await RepositoryContext.SaveChangesAsync(cancellationToken);

        // Return count of removed entries
        return streamGroupVideoStreams.Count;
    }

    public async Task SynchronizeChannelGroupsInStreamGroupAsync(int streamGroupId, List<string>? validChannelGroupNames, CancellationToken cancellationToken)
    {
        StreamGroup? streamGroup = await RepositoryContext.StreamGroups.Include(sg => sg.ChannelGroups)
            .ThenInclude(cg => cg.ChannelGroup)
            .FirstOrDefaultAsync(sg => sg.Id == streamGroupId, cancellationToken);

        if (streamGroup == null)
        {
            throw new Exception("StreamGroup not found.");
        }

        if (validChannelGroupNames == null)
        {
            validChannelGroupNames = streamGroup.ChannelGroups.Select(cg => cg.ChannelGroup.Name).ToList();
        }

        // Remove ChannelGroups not in validChannelGroupNames
        List<StreamGroupChannelGroup> channelGroupsToRemove = streamGroup.ChannelGroups
            .Where(cg => !validChannelGroupNames.Contains(cg.ChannelGroup.Name))
            .ToList();

        foreach (StreamGroupChannelGroup? channelGroupToRemove in channelGroupsToRemove)
        {
            streamGroup.ChannelGroups.Remove(channelGroupToRemove);
        }

        // Get list of existing ChannelGroup names to avoid adding duplicates
        List<string> existingChannelGroupNames = streamGroup.ChannelGroups.Select(cg => cg.ChannelGroup.Name).ToList();

        // Add ChannelGroups that are not already in the StreamGroup
        foreach (string name in validChannelGroupNames)
        {
            if (!existingChannelGroupNames.Contains(name))
            {
                ChannelGroup? channelGroupToAdd = await RepositoryContext.ChannelGroups.FirstOrDefaultAsync(cg => cg.Name == name, cancellationToken);
                if (channelGroupToAdd != null)
                {
                    streamGroup.ChannelGroups.Add(new StreamGroupChannelGroup { ChannelGroupId = channelGroupToAdd.Id, StreamGroupId = streamGroupId });
                }
            }
        }

        await RepositoryContext.SaveChangesAsync(cancellationToken);
    }

    private void UpdateStreamGroup(StreamGroup StreamGroup)
    {
        Update(StreamGroup);
    }

    public async Task<PagedResponse<StreamGroupDto>> GetStreamGroupDtosPagedAsync(StreamGroupParameters StreamGroupParameters, string Url)
    {
        return await GetEntitiesAsync<StreamGroupDto>(StreamGroupParameters, _mapper);
    }

    public async Task<List<VideoStreamDto>> GetStreamGroupVideoStreams(int id, CancellationToken cancellationToken = default)
    {
        if (id == 0)
        {
            return new();
        }

        StreamGroup? streamGroup = await GetStreamGroupWithRelatedEntitiesByIdAsync(id, cancellationToken);

        if (streamGroup == null)
        {
            return new();
        }

        List<VideoStream> cvs = streamGroup.ChildVideoStreams.Select(a => a.ChildVideoStream).ToList();
        List<VideoStreamDto> ret = _mapper.Map<List<VideoStreamDto>>(cvs);

        List<string> existingIds = ret.Select(a => a.Id).ToList();

        List<string> cgNames = streamGroup.ChannelGroups.Select(a => a.ChannelGroup.Name).ToList();

        var streams = await RepositoryContext.VideoStreams
                .Where(a => !existingIds.Contains(a.Id) && cgNames.Contains(a.User_Tvg_group))
                .Select(a => a.Id)
                .AsNoTracking().ToListAsync();

        if (streams.Any())
        {
            List<VideoStreamDto> streamsDto = _mapper.Map<List<VideoStreamDto>>(streams);
            ret.AddRange(streamsDto);
        }

        var links = await RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id).ToListAsync(cancellationToken: cancellationToken);

        foreach (var stream in ret)
        {
            stream.Rank = links.Single(a => a.ChildVideoStreamId == stream.Id).Rank;
        }

        return ret;
    }

    public async Task RemoveVideoStreamFromStreamGroup(int StreamGroupId, string VideoStreamId, CancellationToken cancellationToken = default)
    {
        try
        {
            StreamGroup? streamGroup = await GetStreamGroupByIdAsync(StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return;
            }

            StreamGroupDto? streamGroupDto = await GetStreamGroupDto(StreamGroupId, "", cancellationToken).ConfigureAwait(false);

            if (streamGroupDto == null)
            {
                return;
            }

            var sgVs = await RepositoryContext.StreamGroupVideoStreams
                 .Where(a => a.StreamGroupId == StreamGroupId && a.ChildVideoStreamId != VideoStreamId).AsNoTracking()
                 .Select(a => new VideoStreamIsReadOnly { VideoStreamId = a.ChildVideoStreamId, IsReadOnly = a.IsReadOnly }).ToListAsync(cancellationToken: cancellationToken);

            for (int i = 0; i < sgVs.Count; i++)
            {
                VideoStreamIsReadOnly? s = sgVs[i];
                s.Rank = i;
            }

            await Syncthing(StreamGroupId, "", null, sgVs, cancellationToken);
        }
        catch (Exception)
        {
        }
    }
}