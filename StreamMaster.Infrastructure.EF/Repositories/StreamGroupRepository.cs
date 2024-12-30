using AutoMapper;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Filtering;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupRepository(ILogger<StreamGroupRepository> logger, IRepositoryWrapper Repository, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> intSettings, ICryptoService cryptoService, IHttpContextAccessor httpContextAccessor)
    : RepositoryBase<StreamGroup>(repositoryContext, logger), IStreamGroupRepository
{
    public PagedResponse<StreamGroupDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<StreamGroupDto>(Count());
    }

    public async Task<PagedResponse<StreamGroupDto>> GetPagedStreamGroups(QueryStringParameters Parameters)
    {
        IQueryable<StreamGroup> query = GetQuery(Parameters);

        PagedResponse<StreamGroupDto> ret = await query.GetPagedResponseAsync<StreamGroup, StreamGroupDto>(Parameters.PageNumber, Parameters.PageSize, mapper)
                          .ConfigureAwait(false);

        await SetStreamGroupsLinks(ret.Data!);
        return ret;
    }

    public async Task SetStreamGroupsLinks(List<StreamGroupDto> streamGroupDTOs)
    {
        string Url = httpContextAccessor.GetUrl();

        foreach (StreamGroupDto sg in streamGroupDTOs)
        {
            await SetStreamGroupLinks(sg, Url);
        }
    }

    private async Task SetStreamGroupsLink(StreamGroupDto streamGroupDto)
    {
        string Url = httpContextAccessor.GetUrl();
        await SetStreamGroupLinks(streamGroupDto, Url);
    }

    private async Task SetStreamGroupLinks(StreamGroupDto streamGroupDto, string Url)
    {
        Setting Settings = intSettings.CurrentValue;
        bool sgExists = await Repository.StreamGroup.GetQuery().AnyAsync(a => a.Id == streamGroupDto.Id);
        if (!sgExists)
        {
            return;
        }

        if (streamGroupDto.StreamGroupProfiles.Count > 0)
        {
            foreach (StreamGroupProfileDto streamGroupProfile in streamGroupDto.StreamGroupProfiles)
            {
                string? EncodedString = cryptoService.EncodeInt(streamGroupProfile.Id);
                if (EncodedString == null)
                {
                    continue;
                }

                streamGroupProfile.ShortHDHRLink = $"{Url}/s/{streamGroupProfile.Id}";
                streamGroupProfile.ShortM3ULink = $"{Url}/s/{streamGroupProfile.Id}.m3u";
                streamGroupProfile.ShortEPGLink = $"{Url}/s/{streamGroupProfile.Id}.xml";

                streamGroupProfile.HDHRLink = $"{Url}/api/streamgroups/{EncodedString}";
                streamGroupProfile.M3ULink = $"{Url}/api/streamgroups/{EncodedString}/m3u.m3u";
                streamGroupProfile.XMLLink = $"{Url}/api/streamgroups/{EncodedString}/epg.xml";
            }

            StreamGroupProfileDto defaultProfile = streamGroupDto.StreamGroupProfiles.First(a => a.ProfileName.EqualsIgnoreCase("default"));

            streamGroupDto.ShortHDHRLink = $"{Url}/s/{defaultProfile.Id}";
            streamGroupDto.ShortM3ULink = $"{Url}/s/{defaultProfile.Id}.m3u";
            streamGroupDto.ShortEPGLink = $"{Url}/s/{defaultProfile.Id}.xml";

            streamGroupDto.HDHRLink = defaultProfile.HDHRLink;
            streamGroupDto.M3ULink = defaultProfile.M3ULink;
            streamGroupDto.XMLLink = defaultProfile.XMLLink;
        }
    }

    public async Task<StreamGroupDto?> GetStreamGroupByName(string Name)
    {
        if (string.IsNullOrEmpty(Name))
        {
            return null;
        }

        StreamGroup? streamGroup = await GetQuery(c => c.Name == Name)
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

    public async Task<StreamGroupDto?> GetStreamGroupByIdAsync(int streamGroupId)
    {
        if (streamGroupId == 0)
        {
            return null;
        }

        StreamGroup? streamGroup = await GetQuery(c => c.Id == streamGroupId)
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
        return GetQuery();
    }

    public IQueryable<StreamGroup> GetAllStreamGroupsWithChannelGroups()
    {
        return GetQuery().Include(sg => sg.ChannelGroups)
            .ThenInclude(a => a.ChannelGroup).OrderBy(p => p.Name);
    }

    public async Task<List<StreamGroupDto>> GetStreamGroups(CancellationToken cancellationToken)
    {
        List<StreamGroup> sgs = await GetQuery().OrderBy(a => a.Name).ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        List<StreamGroupDto> ret = mapper.Map<List<StreamGroupDto>>(sgs);

        await SetStreamGroupsLinks(ret);
        return ret;
    }

    public void CreateStreamGroup(StreamGroup StreamGroup)
    {
        Create(StreamGroup);
    }

    public async Task<int?> DeleteStreamGroup(int StreamGroupId)
    {
        IQueryable<StreamGroupChannelGroup> channelGroups = RepositoryContext.StreamGroupChannelGroups.Where(a => a.StreamGroupId == StreamGroupId);

        IQueryable<StreamGroupSMChannelLink> smChannels = RepositoryContext.StreamGroupSMChannelLinks.Where(a => a.StreamGroupId == StreamGroupId);

        IQueryable<StreamGroupProfile> profiles = RepositoryContext.StreamGroupProfiles.Where(a => a.StreamGroupId == StreamGroupId);

        RepositoryContext.StreamGroupChannelGroups.RemoveRange(channelGroups);
        RepositoryContext.StreamGroupSMChannelLinks.RemoveRange(smChannels);
        RepositoryContext.StreamGroupProfiles.RemoveRange(profiles);
        _ = await RepositoryContext.SaveChangesAsync();

        StreamGroup? streamGroup = await FirstOrDefaultAsync(c => c.Id == StreamGroupId);
        if (streamGroup == null)
        {
            return null;
        }
        Delete(streamGroup);
        _ = await RepositoryContext.SaveChangesAsync();
        return streamGroup.Id;
    }

    public async Task<StreamGroupDto?> UpdateStreamGroup(int StreamGroupId, string? NewName, string? DeviceID, string? GroupKey, bool? CreateSTRM)
    {
        StreamGroup? streamGroup = await FirstOrDefaultAsync(c => c.Id == StreamGroupId);
        if (streamGroup == null)
        {
            return null;
        }

        if (!string.IsNullOrEmpty(NewName))
        {
            streamGroup.Name = NewName;
        }

        if (!string.IsNullOrEmpty(GroupKey))
        {
            streamGroup.GroupKey = GroupKey;
        }

        if (!string.IsNullOrEmpty(DeviceID))
        {
            streamGroup.DeviceID = DeviceID;
        }

        if (CreateSTRM.HasValue)
        {
            streamGroup.CreateSTRM = CreateSTRM.Value;
        }

        Update(streamGroup);
        _ = await RepositoryContext.SaveChangesAsync();
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
        return GetQuery();
    }

    public StreamGroup? GetStreamGroup(int streamGroupId)
    {
        return FirstOrDefault(a => a.Id == streamGroupId, tracking: false);
    }

    //public override IQueryable<StreamGroup> GetQuery(bool tracking = false)
    //{
    //    return tracking
    //        ? base.GetQuery(tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.StreamGroupProfiles)
    //        : base.GetQuery(tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.StreamGroupProfiles).AsNoTracking();
    //}

    public override IQueryable<StreamGroup> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.StreamGroupProfiles)
            : base.GetQuery(tracking).Include(a => a.StreamGroupProfiles).AsNoTracking();
    }

    public override IQueryable<StreamGroup> GetQuery(QueryStringParameters parameters, bool tracking = false)
    {
        if (string.IsNullOrEmpty(parameters.JSONFiltersString) && string.IsNullOrEmpty(parameters.OrderBy))
        {
            return GetQuery(tracking);
        }

        List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(parameters.JSONFiltersString);
        return GetQuery(filters, parameters.OrderBy, tracking: tracking);
    }
}