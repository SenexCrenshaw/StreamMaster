using AutoMapper;
using AutoMapper.QueryableExtensions;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupRepository(ILogger<StreamGroupRepository> logger, RepositoryContext repositoryContext, IRepositoryWrapper repository, ISortHelper<StreamGroup> StreamGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender, IHttpContextAccessor httpContextAccessor, ISettingsService settingsService) : RepositoryBase<StreamGroup>(repositoryContext, logger), IStreamGroupRepository
{
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
        Setting setting = await settingsService.GetSettingsAsync();

        Parallel.ForEach(streamGroupDtos, sg =>
        {
            SetStreamGroupLinks(sg, Url, setting);
        });
    }

    private void SetStreamGroupLinks(StreamGroupDto streamGroupDto, string Url, Setting setting)
    {
        int count = 0;
        if (streamGroupDto.Id == 1)
        {
            count = RepositoryContext.VideoStreams.Count();
        }
        else
        {
            count = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == streamGroupDto.Id).Count();
        }
        RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == streamGroupDto.Id).Count();
        string encodedStreamGroupNumber = streamGroupDto.Id.EncodeValue128(setting.ServerKey);
        streamGroupDto.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
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

        return streamGroup != null ? mapper.Map<StreamGroupDto>(streamGroup) : null;
    }

    //public async Task<StreamGroup?> GetStreamGroupWithRelatedEntitiesById(int streamGroupId, CancellationToken cancellationToken)
    //{
    //    return await RepositoryContext.StreamGroups
    //        .Include(sg => sg.ChannelGroups)
    //            .ThenInclude(sgcg => sgcg.ChannelGroup)
    //        .Include(sg => sg.ChildVideoStreams)
    //            .ThenInclude(sgvs => sgvs.ChildVideoStream)
    //        .SingleOrDefault(sg => sg.Id == streamGroupId, cancellationToken);
    //}

    //public async Task<StreamGroupDto?> GetStreamGroupDto(int id, CancellationToken cancellationToken = default)
    //{
    //    string Url = httpContextAccessor.GetUrl();
    //    int count = 0;

    //    StreamGroupDto? ret;
    //    if (id == 1)
    //    {
    //        ret = new StreamGroupDto { Id = 0, Name = "All" };
    //        count = RepositoryContext.VideoStreams.Count();
    //    }
    //    else
    //    {
    //        StreamGroup? streamGroup = await GetStreamGroupById(id).ConfigureAwait(false);
    //        if (streamGroup == null)
    //        {
    //            return null;
    //        }
    //        count = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id).Count();
    //        ret = mapper.Map<StreamGroupDto>(streamGroup);
    //    }

    //    Setting setting = await settingsService.GetSettings();
    //    string encodedStreamGroupNumber = ret.Id.EncodeValue128(setting.ServerKey);
    //    ret.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
    //    ret.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
    //    ret.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
    //    ret.StreamCount = count;
    //    return ret;
    //}

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

    public int? DeleteStreamGroup(int streamGroupId)
    {
        StreamGroup? streamGroup = FindByCondition(c => c.Id == streamGroupId).FirstOrDefault();
        if (streamGroup == null)
        {
            return null;
        }
        Delete(streamGroup);
        return streamGroup.Id;
    }

    public StreamGroupDto? UpdateStreamGroup(int StreamGroupId, string newName)
    {
        StreamGroup? streamGroup = FindByCondition(c => c.Id == StreamGroupId).FirstOrDefault();
        if (streamGroup == null)
        {
            return null;
        }
        streamGroup.Name = newName;
        Update(streamGroup);

        return streamGroup != null ? mapper.Map<StreamGroupDto>(streamGroup) : null;
    }
    public void UpdateStreamGroup(StreamGroup StreamGroup)
    {
        Update(StreamGroup);
    }

    Task<int?> IStreamGroupRepository.DeleteStreamGroup(int streamGroupId)
    {
        throw new NotImplementedException();
    }

    public IQueryable<StreamGroup> GetStreamGroupQuery()
    {
        return FindAll();
    }

    //public async Task<PagedResponse<StreamGroupDto>> GetStreamGroupDtosPaged(StreamGroupParameters StreamGroupParameters)
    //{
    //    string Url = httpContextAccessor.GetUrl();
    //    Setting setting = await settingsService.GetSettings();
    //    PagedResponse<StreamGroupDto> ret = await GetEntities(StreamGroupParameters, mapper);
    //    foreach (StreamGroupDto sg in ret.Data)
    //    {
    //        int count = 0;
    //        if (sg.Id == 1)
    //        {
    //            count = RepositoryContext.VideoStreams.Count();
    //        }
    //        else
    //        {
    //            count = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == sg.Id).Count();
    //        }
    //        RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == sg.Id).Count();
    //        string encodedStreamGroupNumber = sg.Id.EncodeValue128(setting.ServerKey);
    //        sg.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
    //        sg.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
    //        sg.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
    //        sg.StreamCount = count;
    //    }


    //    return ret;
    //}

}