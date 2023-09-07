using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.StreamGroupChannelGroups.Commands;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupRepository(RepositoryContext repositoryContext, ISortHelper<StreamGroup> StreamGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender, IHttpContextAccessor httpContextAccessor) : RepositoryBase<StreamGroup>(repositoryContext), IStreamGroupRepository
{
    private readonly ISortHelper<StreamGroup> _StreamGroupSortHelper = StreamGroupSortHelper;
    private readonly IMapper _mapper = mapper;
    private readonly ISender _sender = sender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task AddStreamGroupRequestAsync(AddStreamGroupRequest request, CancellationToken cancellationToken)
    {
        int streamGroupNumber = GetAllStreamGroups().Max(a => a.StreamGroupNumber) + 1;

        StreamGroup streamGroup = new()
        {
            Name = request.Name,
            StreamGroupNumber = request.StreamGroupNumber,
        };

        CreateStreamGroup(streamGroup);
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);

        if (request.ChannelGroupIds != null && request.ChannelGroupIds.Any())
        {
            await _sender.Send(new SyncStreamGroupChannelGroupsRequest(streamGroupNumber, request.ChannelGroupIds), cancellationToken);
        }
    }

    public async Task<StreamGroupDto?> GetStreamGroupDtoByStreamGroupNumber(int streamGroupNumber, CancellationToken cancellationToken = default)
    {
        StreamGroup? sg = await GetStreamGroupByStreamGroupNumberAsync(streamGroupNumber).ConfigureAwait(false);
        return sg is not null ? await GetStreamGroupDto(sg?.Id ?? 0, cancellationToken).ConfigureAwait(false) : null;
    }

    public async Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken)
    {
        _ = await RepositoryContext.VideoStreams
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
        _ = await RepositoryContext.StreamGroupChannelGroups.AddAsync(streamGroupChannelGroup, cancellationToken);

        // Save changes in database
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);

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

    public async Task<StreamGroup?> GetStreamGroupWithRelatedEntitiesByIdAsync(int streamGroupId, CancellationToken cancellationToken)
    {
        return await RepositoryContext.StreamGroups
            .Include(sg => sg.ChannelGroups)
                .ThenInclude(sgcg => sgcg.ChannelGroup)
            .Include(sg => sg.ChildVideoStreams)
                .ThenInclude(sgvs => sgvs.ChildVideoStream)
            .SingleOrDefaultAsync(sg => sg.Id == streamGroupId, cancellationToken);
    }

    public async Task<StreamGroupDto?> GetStreamGroupDto(int id, CancellationToken cancellationToken = default)
    {
        if (id == 0)
        {
            return new StreamGroupDto { Id = 0, Name = "All" };
        }
        string Url = _httpContextAccessor.GetUrl();

        StreamGroup? streamGroup = await GetStreamGroupWithRelatedEntitiesByIdAsync(id, cancellationToken);

        if (streamGroup == null)
        {
            return null;
        }

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
        return FindAll();
    }

    public IQueryable<StreamGroup> GetAllStreamGroupsWithChannelGroups()
    {
        return FindAll().Include(sg => sg.ChannelGroups)
            .ThenInclude(sgcg => sgcg.ChannelGroup).OrderBy(p => p.Name);
    }

    public async Task<List<StreamGroupDto>> GetStreamGroupDtos(CancellationToken cancellationToken = default)
    {
        List<StreamGroupDto> ret = new();

        foreach (int streamGroupId in GetAllStreamGroups().Select(a => a.Id))
        {
            StreamGroupDto? streamGroup = await GetStreamGroupDto(streamGroupId, cancellationToken);
            if (streamGroup == null)
            {
                continue;
            }

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
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<StreamGroupDto?> UpdateStreamGroupAsync(UpdateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        try
        {
            string Url = _httpContextAccessor.GetUrl();
            StreamGroup? streamGroup = await GetStreamGroupByIdAsync(request.StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                streamGroup.Name = request.Name;
            }

            //if (request.StreamGroupNumber != null)
            //{
            //    if (!await RepositoryContext.StreamGroups.AnyAsync(a => a.StreamGroupNumber == (int)request.StreamGroupNumber, cancellationToken: cancellationToken).ConfigureAwait(false))
            //    {
            //        streamGroup.StreamGroupNumber = (int)request.StreamGroupNumber;
            //    }
            //}

            UpdateStreamGroup(streamGroup);

            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);


            StreamGroupDto? ret = await GetStreamGroupDto(streamGroup.Id, cancellationToken);
            return ret;
        }
        catch (Exception)
        {
            return null;
        }
    }

    //public async Task<StreamGroupDto?> Sync(int streamGroupId, List<string>? ChannelGroupNames, List<VideoStreamIsReadOnly>? VideoStreams, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        StreamGroupDto? streamGroupDto = await GetStreamGroupDto(streamGroupId, cancellationToken).ConfigureAwait(false);

    //        if (streamGroupDto == null)
    //        {
    //            return null;
    //        }

    //        await SynchronizeStreamGroupChannelsAndVideoStreams(streamGroupDto, ChannelGroupNames, VideoStreams, cancellationToken);

    //        StreamGroupDto? ret = await GetStreamGroupDto(streamGroupId, cancellationToken).ConfigureAwait(false);
    //        return ret;
    //    }
    //    catch (Exception)
    //    {
    //        return null;
    //    }
    //}

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
                _ = await RemoveChildVideoStreamsFromStreamGroupAsync(streamGroup.Id, toRemove.Select(a => a.Id).ToList(), cancellationToken).ConfigureAwait(false);
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
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);

        List<StreamGroupVideoStream> toAdd = new();
        for (int i = 0; i < validVideoStreams.Count; i++)
        {
            VideoStreamIsReadOnly? item = validVideoStreams[i];
            VideoStream? stream = RepositoryContext.VideoStreams.FirstOrDefault(a => a.Id == item.VideoStreamId);
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
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);

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
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);

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

        validChannelGroupNames ??= streamGroup.ChannelGroups.Select(cg => cg.ChannelGroup.Name).ToList();

        // Remove ChannelGroups not in validChannelGroupNames
        List<StreamGroupChannelGroup> channelGroupsToRemove = streamGroup.ChannelGroups
            .Where(cg => !validChannelGroupNames.Contains(cg.ChannelGroup.Name))
            .ToList();

        foreach (StreamGroupChannelGroup? channelGroupToRemove in channelGroupsToRemove)
        {
            _ = streamGroup.ChannelGroups.Remove(channelGroupToRemove);
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

        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);
    }

    private void UpdateStreamGroup(StreamGroup StreamGroup)
    {
        Update(StreamGroup);
    }

    public async Task<PagedResponse<StreamGroupDto>> GetStreamGroupDtosPagedAsync(StreamGroupParameters StreamGroupParameters)
    {
        string Url = _httpContextAccessor.GetUrl();
        Setting _setting = FileUtil.GetSetting();
        PagedResponse<StreamGroupDto> ret = await GetEntitiesAsync<StreamGroupDto>(StreamGroupParameters, _mapper);
        foreach (StreamGroupDto sg in ret.Data)
        {
            string encodedStreamGroupNumber = sg.StreamGroupNumber.EncodeValue128(_setting.ServerKey);
            sg.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
            sg.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
            sg.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
        }


        return ret;
    }

}