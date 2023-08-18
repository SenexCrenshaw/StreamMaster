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

    //public IEnumerable<ChannelGroupStreamCount> GetChannelGroupVideoStreamCounts()
    //{
    //    return _memoryCache.ChannelGroupStreamCounts();
    //}

    //public ChannelGroupStreamCount? GetChannelGroupVideoStreamCount(int id)
    //{
    //    return GetChannelGroupVideoStreamCounts().FirstOrDefault(a => a.Id == id);
    //}

    //public async Task AddOrUpdateChannelGroupVideoStreamCounts(List<ChannelGroupStreamCount> channelGroupStreamCounts)
    //{
    //    foreach (ChannelGroupStreamCount item in channelGroupStreamCounts)
    //    {
    //        await AddOrUpdateChannelGroupVideoStreamCount(item, true).ConfigureAwait(false);
    //    }

    //    await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
    //}
    //public async Task ChannelGroupRemoveCount(int Id)
    //{
    //    ChannelGroupStreamCount? count = await RepositoryContext.ChannelGroupStreamCounts.FirstOrDefaultAsync(a => a.Id == Id).ConfigureAwait(false);
    //    if (count != null)
    //    {

    //        RepositoryContext.ChannelGroupStreamCounts.Remove(count);

    //        await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);

    //    }
    //}

    //public async Task ChannelGroupCreateEmptyCount(int Id)
    //{
    //    ChannelGroupStreamCount? count = await RepositoryContext.ChannelGroupStreamCounts.FirstOrDefaultAsync(a => a.Id == Id).ConfigureAwait(false);
    //    if (count == null)
    //    {
    //        count = new ChannelGroupStreamCount { Id = Id };
    //        RepositoryContext.ChannelGroupStreamCounts.Add(count);

    //        await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);

    //    }
    //}


    //public async Task AddOrUpdateChannelGroupVideoStreamCount(ChannelGroupStreamCount response, bool ignoreSave = false)
    //{

    //    ChannelGroupStreamCount? data = RepositoryContext.ChannelGroupStreamCounts.FirstOrDefault(a => a.Id == response.Id);

    //    if (data == null)
    //    {
    //        RepositoryContext.ChannelGroupStreamCounts.Add(response);
    //    }
    //    else
    //    {
    //        RepositoryContext.ChannelGroupStreamCounts.Remove(data);
    //        data.TotalCount = response.TotalCount;
    //        data.ActiveCount = response.ActiveCount;
    //        data.HiddenCount = response.HiddenCount;

    //        RepositoryContext.ChannelGroupStreamCounts.Update(data);
    //    }

    //    if (!ignoreSave)
    //    {
    //        await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
    //    }

    //}
    //public async Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken)
    //{
    //    ChannelGroup? channelGroup = await GetChannelGroupByNameAsync(request.ChannelGroupName).ConfigureAwait(false);

    //    if (channelGroup == null)
    //    {
    //        return (null, null, null);
    //    }

    //    List<VideoStreamDto> beforeResults = RepositoryContext.VideoStreams
    //        .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroup.Name)
    //        .AsNoTracking()
    //        .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

    //    IEnumerable<VideoStreamDto> beforeRegexStreams = await _sender.Send(new GetVideoStreamsByNamePatternQuery(channelGroup.RegexMatch), cancellationToken).ConfigureAwait(false);
    //    if (beforeRegexStreams != null)
    //    {
    //        List<VideoStreamDto> mapped = _mapper.Map<List<VideoStreamDto>>(beforeRegexStreams);
    //        beforeResults.AddRange(mapped);
    //    }

    //    if (request.Rank != null)
    //    {
    //        channelGroup.Rank = (int)request.Rank;
    //    }

    //    bool isChanged = false;

    //    if (request.IsHidden != null && (bool)request.IsHidden != channelGroup.IsHidden)
    //    {
    //        channelGroup.IsHidden = (bool)request.IsHidden;

    //        int results = await RepositoryContext.VideoStreams.
    //        .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroup.Name)
    //            .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, (bool)request.IsHidden), cancellationToken: cancellationToken)
    //            .ConfigureAwait(false);
    //        int whichWay = (bool)request.IsHidden ? -1 * results : results;
    //        await ChannelGroupSetCount(channelGroup.Id, whichWay).ConfigureAwait(false);
    //        isChanged = true;
    //    }

    //    if (!string.IsNullOrEmpty(request.NewGroupName))
    //    {
    //        await RepositoryContext.VideoStreams
    //        .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroup.Name)
    //           .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, request.NewGroupName), cancellationToken: cancellationToken)
    //           .ConfigureAwait(false);

    //        channelGroup.Name = request.NewGroupName;
    //        isChanged = true;
    //    }

    //    if (!string.IsNullOrEmpty(request.Regex))
    //    {
    //        channelGroup.RegexMatch = request.Regex;
    //    }

    //    UpdateChannelGroup(channelGroup);

    //    int retint = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    //    ChannelGroupDto cgresult = _mapper.Map<ChannelGroupDto>(channelGroup);

    //    if (!isChanged)
    //    {
    //        return (cgresult, null, null);
    //    }

    //    List<VideoStreamDto> afterResults = RepositoryContext.VideoStreams
    //       .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroup.Name)
    //    .AsNoTracking()
    //       .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

    //    IEnumerable<VideoStreamDto> afterRegexStreams = await _sender.Send(new GetVideoStreamsByNamePatternQuery(channelGroup.RegexMatch), cancellationToken).ConfigureAwait(false);
    //    if (afterRegexStreams != null)
    //    {
    //        List<VideoStreamDto> mapped = _mapper.Map<List<VideoStreamDto>>(afterRegexStreams);
    //        afterResults.AddRange(mapped);
    //    }

    //    List<VideoStreamDto> distinctList = beforeResults ?? new List<VideoStreamDto>();

    //    if (afterResults is not null)
    //    {
    //        HashSet<string> existingIds = new(distinctList.Select(a => a.Id));
    //        IEnumerable<VideoStreamDto> diff = afterResults.Where(a => !existingIds.Contains(a.Id));
    //        distinctList = distinctList.Concat(diff).ToList();
    //    }

    //    List<StreamGroupDto> streamGroups = new();
    //    if (distinctList.Any())
    //    {
    //        IEnumerable<StreamGroupDto> intStreamGroups = await _sender.Send(new GetStreamGroupsByVideoStreamIdsQuery(distinctList.Select(a => a.Id).ToList(), url), cancellationToken).ConfigureAwait(false);
    //        if (intStreamGroups != null && intStreamGroups.Any())
    //        {
    //            foreach (StreamGroupDto streamGroup in intStreamGroups)
    //            {
    //                UpdateStreamGroupRequest updateStreamGroupRequest = new(
    //                    StreamGroupId: streamGroup.Id,
    //                    Name: null,
    //                    StreamGroupNumber: null,
    //                    VideoStreams: null,
    //                    ChannelGroupNames: streamGroup.ChannelGroups.Select(a => a.Name).ToList()
    //                    );

    //                StreamGroupDto? res = await _sender.Send(updateStreamGroupRequest, cancellationToken).ConfigureAwait(false);
    //                if (res is not null)
    //                {
    //                    streamGroups.Add(res);
    //                }
    //            }
    //        }
    //    }

    //    return (cgresult, distinctList, streamGroups);
    //}

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

#if HAS_REGEX
        List<string> results = new() { videoStreamDto.User_Tvg_group };


        List<ChannelGroup> channelGroups = RepositoryContext.ChannelGroups.Where(a => a.RegexMatch != null && a.RegexMatch != string.Empty)
            .AsNoTracking()
            .ToList();

        foreach (ChannelGroup? cg in channelGroups)
        {
            Regex regex = new(cg.RegexMatch, RegexOptions.ECMAScript | RegexOptions.IgnoreCase);
            if (regex.IsMatch(videoStreamDto.User_Tvg_name))
            {
                results.Add(cg.Name);
            }
        }


        stopwatch.Stop();
        _logger.LogInformation($"GetChannelNamesFromVideoStream took {stopwatch.ElapsedMilliseconds} ms");
        return results;
#else
        stopwatch.Stop();
        _logger.LogInformation($"GetChannelNamesFromVideoStream took {stopwatch.ElapsedMilliseconds} ms");
        return new List<ChannelGroup>() { channelGroup };
#endif
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