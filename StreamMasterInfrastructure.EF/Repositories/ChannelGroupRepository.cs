using AutoMapper;

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

public class ChannelGroupRepository : RepositoryBase<ChannelGroup>, IChannelGroupRepository
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ISender _sender;
    private readonly ILogger _logger;
    public ChannelGroupRepository(ILogger<ChannelGroupRepository> logger, RepositoryContext repositoryContext, IMapper mapper, IMemoryCache memoryCache, ISender sender) : base(repositoryContext)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _logger = logger;
        _sender = sender;
    }

    public IQueryable<ChannelGroup> GetAllChannelGroups()
    {
        return FindAll();
    }

    public IEnumerable<ChannelGroupIdName> GetAllChannelGroupNames()
    {
        return FindAll().OrderBy(a => a.Name).Select(a => new ChannelGroupIdName { Name = a.Name, Id = a.Id });
    }

    public async Task<PagedResponse<ChannelGroupDto>> GetChannelGroupsAsync(ChannelGroupParameters channelGroupParameters)
    {
        PagedResponse<ChannelGroupDto> channelGroups = await GetEntitiesAsync<ChannelGroupDto>(channelGroupParameters, _mapper).ConfigureAwait(false);
        IEnumerable<ChannelGroupStreamCount> actives = _memoryCache.ChannelGroupStreamCounts();

        foreach (ChannelGroupStreamCount? active in actives)
        {
            ChannelGroupDto? dto = channelGroups.Data.FirstOrDefault(a => a.Id == active.Id);
            if (dto == null)
            {
                continue;
            }
            dto.ActiveCount = active.ActiveCount;
            dto.HiddenCount = active.HiddenCount;
            dto.TotalCount = active.TotalCount;
        }

        return channelGroups;
    }

    private async Task<ChannelGroup?> GetChannelGroupFromVideoStream(string channelGroupName, CancellationToken cancellationToken)
    {
        return await GetChannelGroupByNameAsync(channelGroupName).ConfigureAwait(false);
    }

    public async Task<string?> GetChannelGroupNameFromVideoStream(string videoStreamId, CancellationToken cancellationToken)
    {

        VideoStream? videoStream = await RepositoryContext.VideoStreams.FirstOrDefaultAsync(a => a.Id == videoStreamId, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (videoStream == null)
        {
            return null;
        }

        return videoStream.User_Tvg_name;
    }

    public async Task<ChannelGroupDto?> GetChannelGroupFromVideoStreamId(string VideoStreamId, CancellationToken cancellationToken)
    {
        VideoStream? videoStream = await RepositoryContext.VideoStreams.FirstOrDefaultAsync(a => a.Id == VideoStreamId, cancellationToken: cancellationToken);

        if (videoStream == null)
        {
            return null;
        }

        ChannelGroup? res = await GetChannelGroupFromVideoStream(videoStream.User_Tvg_group, cancellationToken).ConfigureAwait(false);

        ChannelGroupDto? ret = _mapper.Map<ChannelGroupDto?>(res);

        return ret;
    }

    public async Task<int?> GetChannelGroupIdFromVideoStream(string channelGroupName, CancellationToken cancellationToken)
    {
        ChannelGroup? res = await GetChannelGroupFromVideoStream(channelGroupName, cancellationToken).ConfigureAwait(false);

        return res?.Id;
    }

    public async Task<ChannelGroup?> GetChannelGroupById(int Id)
    {
        return await FindByCondition(channelGroup => channelGroup.Id == Id).FirstOrDefaultAsync();
    }

    public async Task<ChannelGroupDto?> GetChannelGroupAsync(int Id, CancellationToken cancellationToken = default)
    {
        ChannelGroup? res = await FindByCondition(channelGroup => channelGroup.Id == Id).FirstOrDefaultAsync();
        if (res == null)
        {
            return null;
        }
        ChannelGroupDto dtos = _mapper.Map<ChannelGroupDto>(res);

        return dtos;
    }

    public Task<List<ChannelGroup>> GetChannelGroupsFromNames(List<string> m3uChannelGroupNames)
    {
        IQueryable<ChannelGroup> res = FindByCondition(channelGroup => m3uChannelGroupNames.Contains(channelGroup.Name));
        if (res == null)
        {
            return Task.FromResult(new List<ChannelGroup>());
        }
        return Task.FromResult(res.ToList());
    }

    public async Task<ChannelGroup?> GetChannelGroupByNameAsync(string name)
    {
        return await FindByCondition(channelGroup => channelGroup.Name.Equals(name)).FirstOrDefaultAsync();

    }

    public async Task<IEnumerable<ChannelGroup>> GetAllChannelGroupsAsync()
    {
        return await FindAll()
                        .OrderBy(p => p.Id)
                        .ToListAsync();
    }

    public void CreateChannelGroup(ChannelGroup ChannelGroup)
    {
        Create(ChannelGroup);

    }

    public async Task<(int? ChannelGroupId, IEnumerable<VideoStreamDto> VideoStreams)> DeleteChannelGroup(ChannelGroup ChannelGroup)
    {
        List<VideoStreamDto> vgs = await _sender.Send(new GetVideoStreamsForChannelGroups([ChannelGroup.Id]));
        Delete(ChannelGroup);
        await RepositoryContext.SaveChangesAsync();
        return (ChannelGroup.Id, vgs);
    }

    public void UpdateChannelGroup(ChannelGroup ChannelGroup)
    {
        Update(ChannelGroup);
    }
    public async Task<(IEnumerable<int> ChannelGroupIds, IEnumerable<VideoStreamDto> VideoStreams)> DeleteAllChannelGroupsFromParameters(ChannelGroupParameters Parameters, CancellationToken cancellationToken)
    {
        IQueryable<ChannelGroup> toDelete = GetIQueryableForEntity(Parameters).Where(a => !a.IsReadOnly);
        List<int> ret = toDelete.Select(a => a.Id).ToList();
        List<VideoStreamDto> videoStreams = await _sender.Send(new GetVideoStreamsForChannelGroups(ret), cancellationToken).ConfigureAwait(false);
        await RepositoryContext.BulkDeleteAsync(toDelete, cancellationToken: cancellationToken).ConfigureAwait(false);

        return (ret, videoStreams);
    }

    public Task<List<ChannelGroupDto>> GetChannelGroupsFromVideoStreamIds(IEnumerable<string> VideoStreamIds, CancellationToken cancellationToken)
    {
        IQueryable<VideoStream> videoStreams = RepositoryContext.VideoStreams.Where(a => VideoStreamIds.Contains(a.Id));

        //if (!videoStreams.Any())
        //{
        //    return new();
        //}

        IQueryable<string> channeNames = videoStreams.Select(a => a.User_Tvg_group).Distinct();
        IQueryable<ChannelGroup> res = FindByCondition(a => channeNames.Contains(a.Name));

        List<ChannelGroupDto> ret = _mapper.Map<List<ChannelGroupDto>>(res);

        return Task.FromResult(ret);
    }

    public PagedResponse<ChannelGroupDto> CreateEmptyPagedResponse(ChannelGroupParameters parameters)
    {
        return CreateEmptyPagedResponse<ChannelGroupDto>(parameters);
    }
}