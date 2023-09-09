using AutoMapper;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

using StreamMasterApplication.StreamGroupChannelGroups.Commands;

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
        ret.StreamCount = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == ret.Id).Count();
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
        string Url = _httpContextAccessor.GetUrl();
        Setting _setting = FileUtil.GetSetting();
        PagedResponse<StreamGroupDto> ret = await GetEntitiesAsync<StreamGroupDto>(StreamGroupParameters, _mapper);
        foreach (StreamGroupDto sg in ret.Data)
        {
            string encodedStreamGroupNumber = sg.StreamGroupNumber.EncodeValue128(_setting.ServerKey);
            sg.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
            sg.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
            sg.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
            sg.StreamCount = RepositoryContext.StreamGroupVideoStreams.Where(a => a.StreamGroupId == sg.Id).Count();
        }


        return ret;
    }

}