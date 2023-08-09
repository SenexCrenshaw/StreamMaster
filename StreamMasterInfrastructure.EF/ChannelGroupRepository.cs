using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.StreamGroups.Queries;
using StreamMasterApplication.VideoStreams.Queries;

using StreamMasterDomain.Dto;
using StreamMasterDomain.Filtering;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

using StreamMasterInfrastructureEF.Helpers;

using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Text.Json.Nodes;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace StreamMasterInfrastructureEF;

public class ChannelGroupRepository : RepositoryBase<ChannelGroup>, IChannelGroupRepository
{
    private readonly ISortHelper<ChannelGroup> _channelGroupSortHelper;
    private readonly IMemoryCache _memoryCache;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public ChannelGroupRepository(RepositoryContext repositoryContext, ISortHelper<ChannelGroup> ChannelGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender) : base(repositoryContext)
    {
        _sender = sender;
        _memoryCache = memoryCache;
        _mapper = mapper;
        _channelGroupSortHelper = ChannelGroupSortHelper;
    }

    public async Task<(ChannelGroupDto? channelGroup, List<VideoStreamDto>? distinctList, List<StreamGroupDto>? streamGroupIds)> UpdateChannelGroup(UpdateChannelGroupRequest request, string url, CancellationToken cancellationToken)
    {
        ChannelGroup? channelGroup = await GetChannelGroupByNameAsync(request.GroupName).ConfigureAwait(false);

        if (channelGroup == null)
        {
            return (null, null, null);
        }

        List<VideoStreamDto> beforeResults = RepositoryContext.VideoStreams
            .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
            .AsNoTracking()
            .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        IEnumerable<VideoStream> beforeRegexStreams = await _sender.Send(new GetVideoStreamsByNamePatternQuery(channelGroup.RegexMatch), cancellationToken).ConfigureAwait(false);
        if (beforeRegexStreams != null)
        {
            List<VideoStreamDto> mapped = _mapper.Map<List<VideoStreamDto>>(beforeRegexStreams);
            beforeResults.AddRange(mapped);
        }

        if (request.Rank != null)
        {
            channelGroup.Rank = (int)request.Rank;
        }

        bool isChanged = false;

        if (request.IsHidden != null)
        {
            channelGroup.IsHidden = (bool)request.IsHidden;
            await RepositoryContext.VideoStreams
            .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsHidden, (bool)request.IsHidden), cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.NewGroupName))
        {
            await RepositoryContext.VideoStreams
            .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
               .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, request.NewGroupName), cancellationToken: cancellationToken)
               .ConfigureAwait(false);

            channelGroup.Name = request.NewGroupName;
            isChanged = true;
        }

        if (!string.IsNullOrEmpty(request.Regex))
        {
            channelGroup.RegexMatch = request.Regex;
        }

        Update(channelGroup);

        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        ChannelGroupDto cgresult = _mapper.Map<ChannelGroupDto>(channelGroup);

        if (!isChanged)
        {
            return (cgresult, null, null);
        }

        List<VideoStreamDto> afterResults = RepositoryContext.VideoStreams
           .Where(a => a.User_Tvg_group != null && a.User_Tvg_group.ToLower() == channelGroup.Name.ToLower())
        .AsNoTracking()
           .ProjectTo<VideoStreamDto>(_mapper.ConfigurationProvider).ToList();

        IEnumerable<VideoStream> afterRegexStreams = await _sender.Send(new GetVideoStreamsByNamePatternQuery(channelGroup.RegexMatch), cancellationToken).ConfigureAwait(false);
        if (afterRegexStreams != null)
        {
            List<VideoStreamDto> mapped = _mapper.Map<List<VideoStreamDto>>(afterRegexStreams);
            afterResults.AddRange(mapped);
        }

        List<VideoStreamDto> distinctList = beforeResults ?? new List<VideoStreamDto>();

        if (afterResults is not null)
        {
            HashSet<string> existingIds = new(distinctList.Select(a => a.Id));
            IEnumerable<VideoStreamDto> diff = afterResults.Where(a => !existingIds.Contains(a.Id));
            distinctList = distinctList.Concat(diff).ToList();
        }

        List<StreamGroupDto> streamGroups = new();
        if (distinctList.Any())
        {
            IEnumerable<StreamGroupDto> intStreamGroups = await _sender.Send(new GetStreamGroupsByVideoStreamIdsQuery(distinctList.Select(a => a.Id).ToList(), url), cancellationToken).ConfigureAwait(false);
            if (intStreamGroups != null && intStreamGroups.Any())
            {
                foreach (StreamGroupDto streamGroup in intStreamGroups)
                {
                    UpdateStreamGroupRequest updateStreamGroupRequest = new(
                        StreamGroupId: streamGroup.Id,
                        Name: null,
                        StreamGroupNumber: null,
                        VideoStreams: null,
                        ChannelGroupNames: streamGroup.ChannelGroups.Select(a => a.Name).ToList()
                        );

                    StreamGroupDto? res = await _sender.Send(updateStreamGroupRequest, cancellationToken).ConfigureAwait(false);
                    if (res is not null)
                    {
                        streamGroups.Add(res);
                    }
                }
            }
        }

        return (cgresult, distinctList, streamGroups);
    }

    public IQueryable<ChannelGroup> GetAllChannelGroups()
    {
        return FindAll().OrderBy(p => p.Name);
    }

    public async Task<PagedResponse<ChannelGroupDto>> GetChannelGroupsAsync(ChannelGroupParameters channelGroupParameters)
    {
        var result = await GetEntitiesAsync<ChannelGroupDto>(channelGroupParameters, _mapper);

        return result;
    }

    public async Task<ChannelGroup?> GetChannelGroupAsync(int Id)
    {
        return await FindByCondition(channelGroup => channelGroup.Id == Id).FirstOrDefaultAsync();
    }

    public async Task<ChannelGroup?> GetChannelGroupByNameAsync(string name)
    {
        return await FindByCondition(channelGroup => channelGroup.Name.ToLower().Equals(name.ToLower()))
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