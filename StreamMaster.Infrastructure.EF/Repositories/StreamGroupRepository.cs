using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Authentication;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Filtering;

using System.Text.Json;
using System.Web;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class StreamGroupRepository(ILogger<StreamGroupRepository> logger, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> intSettings, IHttpContextAccessor httpContextAccessor)
    : RepositoryBase<StreamGroup>(repositoryContext, logger, intSettings), IStreamGroupRepository
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

        SetStreamGroupsLinks(ret.Data);
        return ret;
    }


    public void SetStreamGroupsLinks(List<StreamGroupDto> streamGroupDtos)
    {
        string Url = httpContextAccessor.GetUrl();


        foreach (StreamGroupDto sg in streamGroupDtos)
        {
            SetStreamGroupLinks(sg, Url);
        }
    }

    private async Task SetStreamGroupsLink(StreamGroupDto streamGroupDto)
    {
        string Url = httpContextAccessor.GetUrl();


        SetStreamGroupLinks(streamGroupDto, Url);
    }

    private void SetStreamGroupLinks(StreamGroupDto streamGroupDto, string Url)
    {
        Setting Settings = intSettings.CurrentValue;
        //int count = streamGroupDto.IsReadOnly
        //    ? RepositoryContext.SMStreams.Count()
        //    : RepositoryContext.StreamGroupSMChannelLinks.Where(a => a.StreamGroupId == streamGroupDto.Id).Count();
        if (streamGroupDto.StreamGroupProfiles.Count > 0)
        {
            foreach (var sgprofile in streamGroupDto.StreamGroupProfiles)
            {
                //string encodedStreamGroupNumber = streamGroupDto.Id.EncodeValue128(Settings.ServerKey);
                string encodedStreamGroupNumber = streamGroupDto.Id.EncodeValues128(sgprofile.Id, Settings.ServerKey);

                string encodedName = HttpUtility.HtmlEncode(streamGroupDto.Name).Trim()
                            .Replace("/", "")
                            .Replace(" ", "_");

                sgprofile.M3ULink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/m3u.m3u";
                sgprofile.ShortM3ULink = $"{Url}/v/s/{encodedName}.m3u";
                sgprofile.ShortEPGLink = $"{Url}/v/s/{encodedName}.xml";
                sgprofile.XMLLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}/epg.xml";
                sgprofile.HDHRLink = $"{Url}/api/streamgroups/{encodedStreamGroupNumber}";
            }

            var defaultProfile = streamGroupDto.StreamGroupProfiles.FirstOrDefault(a => a.Name == "Default");
            if (defaultProfile != null)
            {
                streamGroupDto.ShortM3ULink = defaultProfile.ShortM3ULink;
                streamGroupDto.ShortEPGLink = defaultProfile.ShortEPGLink;
                streamGroupDto.M3ULink = defaultProfile.M3ULink;
                streamGroupDto.XMLLink = defaultProfile.XMLLink;
                streamGroupDto.HDHRLink = defaultProfile.HDHRLink;
            }
        }


        //streamGroupDto.StreamCount = count;
    }

    public async Task<StreamGroupDto?> GetStreamGroupById(int streamGroupId)
    {
        if (streamGroupId == 0)
        {
            StreamGroupDto dto = new() { Id = 0, Name = "All", IsReadOnly = true };
            await SetStreamGroupsLink(dto);
            return dto;
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
            .ThenInclude(sgcg => sgcg.ChannelGroup).OrderBy(p => p.Name);
    }

    public async Task<List<StreamGroupDto>> GetStreamGroups(CancellationToken cancellationToken)
    {
        List<StreamGroupDto> ret = await GetQuery()
            .OrderBy(a => a.Name)
                   .ProjectTo<StreamGroupDto>(mapper.ConfigurationProvider)
                   .ToListAsync(cancellationToken: cancellationToken)
                   .ConfigureAwait(false);

        SetStreamGroupsLinks(ret);
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

        var profiles = RepositoryContext.StreamGroupProfiles.Where(a => a.StreamGroupId == StreamGroupId);

        RepositoryContext.StreamGroupChannelGroups.RemoveRange(channelGroups);
        RepositoryContext.StreamGroupSMChannelLinks.RemoveRange(smChannels);
        RepositoryContext.StreamGroupProfiles.RemoveRange(profiles);
        await RepositoryContext.SaveChangesAsync();

        StreamGroup? streamGroup = await FirstOrDefaultAsync(c => c.Id == StreamGroupId);
        if (streamGroup == null)
        {
            return null;
        }
        Delete(streamGroup);
        await RepositoryContext.SaveChangesAsync();
        return streamGroup.Id;
    }

    public async Task<StreamGroupDto?> UpdateStreamGroup(int StreamGroupId, string? NewName, string? DeviceID, bool? AutoSetChannelNumbers, bool? IgnoreExistingChannelNumbers, int? StartingChannelNumber)
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

        if (!string.IsNullOrEmpty(DeviceID))
        {
            streamGroup.DeviceID = DeviceID;
        }

        if (StartingChannelNumber.HasValue)
        {
            streamGroup.StartingChannelNumber = StartingChannelNumber.Value;
        }

        if (IgnoreExistingChannelNumbers.HasValue)
        {
            streamGroup.IgnoreExistingChannelNumbers = IgnoreExistingChannelNumbers.Value;
        }

        if (AutoSetChannelNumbers.HasValue)
        {
            streamGroup.AutoSetChannelNumbers = AutoSetChannelNumbers.Value;
        }


        Update(streamGroup);
        await RepositoryContext.SaveChangesAsync();
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

    public StreamGroup? GetStreamGroup(int streamGrouId)
    {
        return FirstOrDefault(a => a.Id == streamGrouId, tracking: false);
    }

    public async Task<IdIntResultWithResponse> AutoSetSMChannelNumbers(int streamGroupId, int startingNumber, bool overWriteExisting, QueryStringParameters Parameters)
    {
        IdIntResultWithResponse ret = new();

        if (string.IsNullOrEmpty(Parameters.JSONFiltersString))
        {
            ret.APIResponse = APIResponse.ErrorWithMessage("JSONFiltersString null");
            return ret;
        }
        StreamGroup? streamGroup = await GetQuery(true).FirstOrDefaultAsync(a => a.Id == streamGroupId);
        if (streamGroup == null)
        {
            ret.APIResponse = APIResponse.ErrorWithMessage("Stream Group not found");
            return ret;
        }


        List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(Parameters.JSONFiltersString);
        IQueryable<SMChannel> channels = FilterHelper<SMChannel>.ApplyFiltersAndSort(streamGroup.SMChannels.Select(a => a.SMChannel).AsQueryable(), filters, Parameters.OrderBy, true);

        ConcurrentHashSet<int> existingNumbers = [];
        if (!overWriteExisting)
        {
            existingNumbers.UnionWith(streamGroup.SMChannels.Select(a => a.SMChannel.ChannelNumber).Distinct());
        }
        int number = startingNumber;

        foreach (SMChannel channel in channels)
        {
            if (!overWriteExisting && channel.ChannelNumber != 0)
            {
                continue;
            }
            if (overWriteExisting)
            {
                channel.ChannelNumber = number++;
            }
            else
            {
                number = GetNextNumber(number, existingNumbers);
                channel.ChannelNumber = number;
                _ = existingNumbers.Add(number);
            }
            RepositoryContext.SMChannels.Update(channel);
            ret.Add(new IdIntResult { Id = channel.Id, Result = channel });
        }

        await RepositoryContext.SaveChangesAsync();

        return ret;
    }

    private int GetNextNumber(int startNumber, ConcurrentHashSet<int> existingNumbers)
    {
        while (existingNumbers.Contains(startNumber))
        {
            startNumber++;
        }
        return startNumber;
    }
    public override IQueryable<StreamGroup> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.StreamGroupProfiles)
            : base.GetQuery(tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.StreamGroupProfiles).AsNoTracking();
    }

    public override IQueryable<StreamGroup> GetQuery(QueryStringParameters parameters, bool tracking = false)
    {
        // If there are no filters or order specified, just return all entities.
        if (string.IsNullOrEmpty(parameters.JSONFiltersString) && string.IsNullOrEmpty(parameters.OrderBy))
        {
            return GetQuery(tracking);
        }

        List<DataTableFilterMetaData> filters = Utils.GetFiltersFromJSON(parameters.JSONFiltersString);
        return GetQuery(filters, parameters.OrderBy, tracking: tracking);
    }
}