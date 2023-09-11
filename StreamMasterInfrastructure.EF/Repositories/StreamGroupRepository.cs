using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterDomain.Authentication;
using StreamMasterDomain.Common;
using StreamMasterDomain.Dto;
using StreamMasterDomain.Pagination;
using StreamMasterDomain.Repository;
using StreamMasterDomain.Sorting;

namespace StreamMasterInfrastructureEF.Repositories;

public class StreamGroupRepository(RepositoryContext repositoryContext, ISortHelper<StreamGroup> StreamGroupSortHelper, IMapper mapper, IMemoryCache memoryCache, ISender sender, IHttpContextAccessor httpContextAccessor, ISettingsService settingsService) : RepositoryBase<StreamGroup, StreamGroupDto>(repositoryContext), IStreamGroupRepository
{
    public async Task CreateStreamGroupRequestAsync(CreateStreamGroupRequest request, CancellationToken cancellationToken)
    {
        StreamGroup streamGroup = new()
        {
            Name = request.Name
        };

        CreateStreamGroup(streamGroup);
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetGroupNameByGroupName(string channelGroupName, string newGroupName, CancellationToken cancellationToken)
    {
        _ = await RepositoryContext.VideoStreams
              .Where(a => a.User_Tvg_group != null && a.User_Tvg_group == channelGroupName)
              .ExecuteUpdateAsync(s => s.SetProperty(b => b.User_Tvg_group, newGroupName), cancellationToken: cancellationToken)
              .ConfigureAwait(false);
    }

    //public async Task<bool> AddChannelGroupToStreamGroupAsync(int streamGroupId, int channelGroupId, CancellationToken cancellationToken)
    //{
    //    // Check if combination already exists
    //    bool alreadyExists = await RepositoryContext.StreamGroupChannelGroups
    //        .AnyAsync(sgcg => sgcg.StreamGroupId == streamGroupId && sgcg.ChannelGroupId == channelGroupId, cancellationToken);

    //    // If combination exists, return false
    //    if (alreadyExists)
    //    {
    //        return false;
    //    }

    //    // If not, create new StreamGroupChannelGroup entity
    //    StreamGroupChannelGroup streamGroupChannelGroup = new()
    //    {
    //        StreamGroupId = streamGroupId,
    //        ChannelGroupId = channelGroupId
    //    };

    //    // Add new entity to DbSet
    //    _ = await RepositoryContext.StreamGroupChannelGroups.AddAsync(streamGroupChannelGroup, cancellationToken);

    //    // Save changes in database
    //    _ = await RepositoryContext.SaveChangesAsync(cancellationToken);

    //    // Return true indicating successful addition
    //    return true;
    //}

    public async Task<IPagedList<StreamGroup>> GetStreamGroupsAsync(StreamGroupParameters StreamGroupParameters)
    {
        IQueryable<StreamGroup> StreamGroups = FindAll();

        IQueryable<StreamGroup> sorderStreamGroups = StreamGroupSortHelper.ApplySort(StreamGroups, StreamGroupParameters.OrderBy);

        return await sorderStreamGroups.ToPagedListAsync(StreamGroupParameters.PageNumber, StreamGroupParameters.PageSize).ConfigureAwait(false);
    }

    public async Task<StreamGroup?> GetStreamGroupByIdAsync(int id)
    {
        return await FindByCondition(StreamGroup => StreamGroup.Id == id).FirstOrDefaultAsync();
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
        string Url = httpContextAccessor.GetUrl();
        int count = 0;

        StreamGroupDto? ret;
        if (id == 1)
        {
            ret = new StreamGroupDto { Id = 0, Name = "All" };
            count = RepositoryContext.VideoStreams.Count();
        }
        else
        {
            StreamGroup? streamGroup = await GetStreamGroupByIdAsync(id).ConfigureAwait(false);
            if (streamGroup == null)
            {
                return null;
            }
            count = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == id).Count();
            ret = mapper.Map<StreamGroupDto>(streamGroup);
        }

        Setting setting = await settingsService.GetSettingsAsync();
        string encodedStreamGroupNumber = ret.Id.EncodeValue128(setting.ServerKey);
        ret.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
        ret.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
        ret.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
        ret.StreamCount = count;
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
            string Url = httpContextAccessor.GetUrl();
            StreamGroup? streamGroup = await GetStreamGroupByIdAsync(request.StreamGroupId).ConfigureAwait(false);

            if (streamGroup == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                streamGroup.Name = request.Name;
            }
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


    private void UpdateStreamGroup(StreamGroup StreamGroup)
    {
        Update(StreamGroup);
    }

    public async Task<PagedResponse<StreamGroupDto>> GetStreamGroupDtosPagedAsync(StreamGroupParameters StreamGroupParameters)
    {
        string Url = httpContextAccessor.GetUrl();
        Setting setting = await settingsService.GetSettingsAsync();
        PagedResponse<StreamGroupDto> ret = await GetEntitiesAsync(StreamGroupParameters, mapper);
        foreach (StreamGroupDto sg in ret.Data)
        {
            int count = 0;
            if (sg.Id == 1)
            {
                count = RepositoryContext.VideoStreams.Count();
            }
            else
            {
                count = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == sg.Id).Count();
            }
            RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == sg.Id).Count();
            string encodedStreamGroupNumber = sg.Id.EncodeValue128(setting.ServerKey);
            sg.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
            sg.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
            sg.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
            sg.StreamCount = count;
        }


        return ret;
    }

}