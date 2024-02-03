using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMaster.Domain.Authentication;

using System.Web;

namespace StreamMaster.Infrastructure.EF.PSQL.Repositories;

public class StreamGroupRepository(ILogger<StreamGroupRepository> logger, RepositoryContext repositoryContext, IMapper mapper, IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor) : RepositoryBase<StreamGroup>(repositoryContext, logger), IStreamGroupRepository
{
    public PagedResponse<StreamGroupDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<StreamGroupDto>(Count());
    }

    public async Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(StreamGroupParameters Parameters)
    {
        IQueryable<StreamGroup> query = GetIQueryableForEntity(Parameters);
        PagedResponse<StreamGroupDto> ret = await query.GetPagedResponseAsync<StreamGroup, StreamGroupDto>(Parameters.PageNumber, Parameters.PageSize, mapper)
                          .ConfigureAwait(false);

        await SetStreamGroupsLinks(ret.Data).ConfigureAwait(false);
        return ret;
    }

    private async Task SetStreamGroupsLinks(List<StreamGroupDto> streamGroupDtos)
    {
        string Url = httpContextAccessor.GetUrl();
        Setting setting = memoryCache.GetSetting();

        foreach (StreamGroupDto sg in streamGroupDtos)
        {
            SetStreamGroupLinks(sg, Url, setting);
        }
    }

    private async Task SetStreamGroupsLink(StreamGroupDto streamGroupDto)
    {
        string Url = httpContextAccessor.GetUrl();
        Setting setting = memoryCache.GetSetting();

        SetStreamGroupLinks(streamGroupDto, Url, setting);
    }

    private void SetStreamGroupLinks(StreamGroupDto streamGroupDto, string Url, Setting setting)
    {
        int count = streamGroupDto.Id == 1
            ? RepositoryContext.VideoStreams.Count()
            : RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == streamGroupDto.Id).Count();
        string encodedStreamGroupNumber = streamGroupDto.Id.EncodeValue128(setting.ServerKey);

        string encodedName = HttpUtility.HtmlEncode(streamGroupDto.Name).Trim()
                    .Replace("/", "")
                    .Replace(" ", "_");

        streamGroupDto.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
        streamGroupDto.ShortM3ULink = $"{Url}/v/s/{encodedName}.m3u";
        streamGroupDto.ShortEPGLink = $"{Url}/v/s/{encodedName}.xml";
        streamGroupDto.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
        streamGroupDto.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
        streamGroupDto.StreamCount = count;
    }

    public async Task<StreamGroupDto?> GetStreamGroupById(int streamGroupId)
    {
        StreamGroup? streamGroup = await FindByCondition(c => c.Id == streamGroupId)
                            .AsNoTracking()
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

        if (streamGroup == null)
        {
            return null;
        }

        StreamGroupDto ret = mapper.Map<StreamGroupDto>(streamGroup);

        await SetStreamGroupsLink(ret);
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

    public async Task<List<StreamGroupDto>> GetStreamGroups(CancellationToken cancellationToken)
    {
        List<StreamGroupDto> ret = await FindAll()
                   .ProjectTo<StreamGroupDto>(mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken: cancellationToken)
                   .ConfigureAwait(false);

        await SetStreamGroupsLinks(ret).ConfigureAwait(false);
        return ret;
    }

    public void CreateStreamGroup(StreamGroup StreamGroup)
    {
        Create(StreamGroup);
    }

    public async Task<int?> DeleteStreamGroup(int streamGroupId)
    {
        IQueryable<StreamGroupVideoStream> videoStreams = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == streamGroupId);
        IQueryable<StreamGroupChannelGroup> channelGroups = RepositoryContext.StreamGroupChannelGroups.Where(a => a.StreamGroupId == streamGroupId);
        RepositoryContext.StreamGroupVideoStreams.RemoveRange(videoStreams);
        RepositoryContext.StreamGroupChannelGroups.RemoveRange(channelGroups);
        await RepositoryContext.SaveChangesAsync();

        StreamGroup? streamGroup = FindByCondition(c => c.Id == streamGroupId).FirstOrDefault();
        if (streamGroup == null)
        {
            return null;
        }
        Delete(streamGroup);
        await RepositoryContext.SaveChangesAsync();
        return streamGroup.Id;
    }

    public async Task<StreamGroupDto?> UpdateStreamGroup(UpdateStreamGroupRequest request)
    {
        int StreamGroupId = request.StreamGroupId;

        StreamGroup? streamGroup = FindByCondition(c => c.Id == StreamGroupId).FirstOrDefault();
        if (streamGroup == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            streamGroup.Name = request.Name;
        }

        if (request.AutoSetChannelNumbers != null)
        {
            streamGroup.AutoSetChannelNumbers = (bool)request.AutoSetChannelNumbers;
        }

        Update(streamGroup);

        StreamGroupDto ret = mapper.Map<StreamGroupDto>(streamGroup);

        await SetStreamGroupsLink(ret);

        return ret;
    }

    public void UpdateStreamGroup(StreamGroup StreamGroup)
    {
        Update(StreamGroup);
    }

    public IQueryable<StreamGroup> GetStreamGroupQuery()
    {
        return FindAll();
    }
}