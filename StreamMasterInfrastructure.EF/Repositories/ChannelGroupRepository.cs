using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Cache;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;

using System.Diagnostics;
using System.Linq.Dynamic.Core;

namespace StreamMasterInfrastructureEF.Repositories;

public class ChannelGroupRepository : RepositoryBase<ChannelGroup>, IChannelGroupRepository
{
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger _logger;
    public ChannelGroupRepository(ILogger<ChannelGroupRepository> logger, RepositoryContext repositoryContext, IMapper mapper, IMemoryCache memoryCache) : base(repositoryContext)
    {
        _memoryCache = memoryCache;
        _mapper = mapper;
        _logger = logger;
    }

    public IQueryable<ChannelGroup> GetAllChannelGroups()
    {
        return FindAll();
    }

    public IQueryable<string> GetAllChannelGroupNames()
    {
        return FindAll().OrderBy(a => a.Name).Select(a => a.Name);
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

    private async Task<List<ChannelGroup>> GetChannelGroupsFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = new();
        stopwatch.Start();
        ChannelGroup? channelGroup = await GetChannelGroupByNameAsync(videoStreamDto.User_Tvg_group).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return new();
        }

        stopwatch.Stop();
        _logger.LogInformation($"GetChannelNamesFromVideoStream took {stopwatch.ElapsedMilliseconds} ms");
        return new List<ChannelGroup>() { channelGroup };

    }

    public async Task<List<string>> GetChannelNamesFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken)
    {

        List<ChannelGroup> res = await GetChannelGroupsFromVideoStream(videoStreamDto, cancellationToken).ConfigureAwait(false);

        return res.Select(a => a.Name).ToList();

    }

    public async Task<List<int>> GetChannelIdsFromVideoStream(VideoStreamDto videoStreamDto, CancellationToken cancellationToken)
    {
        List<ChannelGroup> res = await GetChannelGroupsFromVideoStream(videoStreamDto, cancellationToken).ConfigureAwait(false);

        return res.Select(a => a.Id).ToList();
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
        return await FindByCondition(channelGroup => channelGroup.Name.Equals(name))
                         .FirstOrDefaultAsync();
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

    public void DeleteChannelGroup(ChannelGroup ChannelGroup)
    {
        Delete(ChannelGroup);
    }

    public void UpdateChannelGroup(ChannelGroup ChannelGroup)
    {
        Update(ChannelGroup);
    }


}