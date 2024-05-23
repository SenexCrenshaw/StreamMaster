using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Exceptions;
using StreamMaster.Domain.Filtering;
using StreamMaster.Infrastructure.EF.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;

using System.Linq.Expressions;
using System.Text.Json;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelsRepository(ILogger<SMChannelsRepository> intLogger, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> intSettings, IIconService iconService, ISchedulesDirectDataService schedulesDirectDataService)
    : RepositoryBase<SMChannel>(repositoryContext, intLogger, intSettings), ISMChannelsRepository
{

    public List<SMChannelDto> GetSMChannels()
    {
        return [.. GetQuery().Include(a => a.SMStreams).ThenInclude(a => a.SMStream).ProjectTo<SMChannelDto>(mapper.ConfigurationProvider)];
    }

    public PagedResponse<SMChannelDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMChannelDto>(Count());
    }

    public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> query = GetQuery(parameters).Include(a => a.SMStreams).ThenInclude(a => a.SMStream);

        if (!string.IsNullOrEmpty(parameters.JSONFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(parameters.JSONFiltersString);
            if (filters != null && filters.Any(a => a.MatchMode == "inSG"))
            {
                DataTableFilterMetaData? inSGFilter = filters.FirstOrDefault(a => a.MatchMode == "inSG");
                if (inSGFilter != null && inSGFilter.Value != null)
                {
                    try
                    {
                        string? streamGroupIdString = inSGFilter.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(streamGroupIdString))
                        {
                            int streamGroupId = Convert.ToInt32(streamGroupIdString);
                            List<int> linkIds = repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId).ToList();
                            query = query.Where(a => linkIds.Contains(a.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Object value is outside the range of an Int32. Object: {Object}", inSGFilter.Value);
                    }

                }
            }

            if (filters != null && filters.Any(a => a.MatchMode == "notInSG"))
            {
                DataTableFilterMetaData? notInSGFilter = filters.FirstOrDefault(a => a.MatchMode == "notInSG");
                if (notInSGFilter != null && notInSGFilter.Value != null)
                {
                    try
                    {
                        string? streamGroupIdString = notInSGFilter.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(streamGroupIdString))
                        {
                            int streamGroupId = Convert.ToInt32(streamGroupIdString);
                            List<int> linkIds = repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId).ToList();
                            query = query.Where(a => !linkIds.Contains(a.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Object value is outside the range of an Int32. Object: {Object}", notInSGFilter.Value);
                    }

                }

            }
        }

        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                              .ConfigureAwait(false);
    }

    public async Task CreateSMChannel(SMChannel sMChannel)
    {
        Create(sMChannel);
        await SaveChangesAsync();
    }

    public async Task<APIResponse> DeleteSMChannel(int smchannelId)
    {
        try
        {
            SMChannel? channel = await FirstOrDefaultAsync(a => a.Id == smchannelId);
            if (channel == null)
            {
                return APIResponse.ErrorWithMessage("SMChannel not found");
            }

            Delete(channel);
            await SaveChangesAsync();
            return APIResponse.OkWithMessage(channel.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannel with id {smchannelId}", smchannelId);
            return APIResponse.ErrorWithMessage(ex, $"Error deleting SMChannel with id {smchannelId}");
        }

    }

    public async Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> toDelete = GetQuery(parameters);
        return await DeleteSMChannelsAsync(toDelete).ConfigureAwait(false);
    }

    public SMChannel? GetSMChannel(int smchannelId)
    {
        return FirstOrDefault(a => a.Id == smchannelId, tracking: false);
    }

    public async Task<APIResponse> CreateSMChannelFromStream(string streamId)
    {
        SMStreamDto? smStream = repository.SMStream.GetSMStream(streamId);
        if (smStream == null)
        {
            throw new APIException($"Stream with Id {streamId} is not found");
        }

        SMChannel smChannel = new()
        {
            ChannelNumber = smStream.ChannelNumber,
            Group = smStream.Group,
            Name = smStream.Name,
            Logo = smStream.Logo,
            EPGId = smStream.EPGID,
            StationId = smStream.StationId
        };

        await CreateSMChannel(smChannel);

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, smStream.Id);
        return APIResponse.Success;
    }

    public async Task<APIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        IQueryable<SMChannel> toDelete = GetQuery(true).Where(a => smchannelIds.Contains(a.Id));
        if (!toDelete.Any())
        {
            return APIResponse.NotFound;
        }

        await DeleteSMChannelsAsync(toDelete);

        return APIResponse.Success;
    }


    [LogExecutionTimeAspect]
    private async Task<List<int>> DeleteSMChannelsAsync(IQueryable<SMChannel> channels)
    {
        if (!channels.Any())
        {
            return [];
        }
        try
        {
            List<SMChannel> a = channels.ToList();
            List<int> ret = [.. a.Select(a => a.Id)];
            IQueryable<SMChannelStreamLink> linksToDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => ret.Contains(a.SMChannelId));
            await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(linksToDelete);
            await SaveChangesAsync();
            BulkDelete(a);
            await SaveChangesAsync();
            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannels");
        }
        return [];
    }

    public async Task<APIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId)
    {
        if (GetSMChannel(SMChannelId) == null || repository.SMStream.GetSMStream(SMStreamId) == null)
        {
            throw new APIException($"Channel with Id {SMChannelId} or stream with Id {SMStreamId} not found");
        }

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(SMChannelId, SMStreamId);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> RemoveSMStreamFromSMChannel(int SMChannelId, string SMStreamId)
    {
        IQueryable<SMChannelStreamLink> toDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => a.SMChannelId == SMChannelId && a.SMStreamId == SMStreamId);
        if (!toDelete.Any())
        {
            throw new APIException($"Channel with id {SMChannelId} does not contain stream with Id {SMStreamId}");
        }
        await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(toDelete);

        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMStreamRanks(List<SMChannelRankRequest> request)
    {
        return await repository.SMChannelStreamLink.SetSMStreamRank(request);

    }

    public async Task<APIResponse> SetSMChannelLogo(int SMChannelId, string logo)
    {
        SMChannel? channel = GetSMChannel(SMChannelId);
        if (channel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel {SMChannelId} doesnt exist");
        }

        channel.Logo = logo;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public override IQueryable<SMChannel> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups)
            : base.GetQuery(tracking).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups).AsNoTracking();
    }

    public override IQueryable<SMChannel> GetQuery(Expression<Func<SMChannel, bool>> expression, bool tracking = false)
    {
        return base.GetQuery(expression, tracking).Include(a => a.SMStreams).ThenInclude(a => a.SMStream);
    }

    public async Task<APIResponse> SetSMChannelChannelNumber(int sMChannelId, int channelNumber)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.ChannelNumber = channelNumber;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }


    public async Task<APIResponse> SetSMChannelName(int sMChannelId, string name)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.Name = name;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMChannelEPGID(int sMChannelId, string EPGId)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.EPGId = EPGId;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMChannelGroup(int sMChannelId, string group)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.Group = group;
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> CopySMChannel(int sMChannelId, string newName)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        SMChannel newChannel = channel.DeepCopy();
        newChannel.Id = 0;

        newChannel.Name = newName;
        await CreateSMChannel(newChannel);
        await SaveChangesAsync();
        List<SMChannelStreamLink> links = repository.SMChannelStreamLink.GetQuery().Where(a => a.SMChannelId == sMChannelId).ToList();

        foreach (SMChannelStreamLink? link in links)
        {
            SMChannelStreamLink newLink = new()
            {
                SMChannelId = newChannel.Id,
                SMStreamId = link.SMStreamId,
                Rank = link.Rank,
            };

            repository.SMChannelStreamLink.Create(newLink);
        }
        await SaveChangesAsync();

        return APIResponse.Success;

    }

    public async Task<APIResponse> CreateSMChannelFromStreams(List<string> streamIds)
    {
        try
        {
            foreach (string streamId in streamIds)
            {
                APIResponse resp = await CreateSMChannelFromStream(streamId);
                if (resp.IsError)
                {
                    return resp;
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SMChannel from streams");
            return APIResponse.ErrorWithMessage(ex, "Error creating SMChannel from streams");
        }

        return APIResponse.Success;
    }

    public Task<APIResponse> CreateSMChannelFromStreamParameters(QueryStringParameters parameters)
    {
        IQueryable<SMStream> toCreate = repository.SMStream.GetQuery(parameters);
        return CreateSMChannelFromStreams(toCreate.Select(a => a.Id).ToList());
    }

    public async Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken)
    {
        List<FieldData> ret = [];
        List<SMChannel> channels = GetQuery(true).Where(a => ids.Contains(a.Id)).ToList();

        foreach (SMChannel? channel in channels)
        {
            channel.IsHidden = !channel.IsHidden;
            ret.Add(new FieldData(() => channel.IsHidden));
        }
        await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return ret;
    }

    public async Task<SMChannelDto?> ToggleSMChannelVisibleById(int id, CancellationToken cancellationToken)
    {
        if (id == 0)
        {
            throw new ArgumentNullException(nameof(id));
        }

        SMChannel? channel = await FirstOrDefaultAsync(a => a.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        if (channel == null)
        {
            return null;
        }
        channel.IsHidden = !channel.IsHidden;
        Update(channel);
        await SaveChangesAsync();
        return mapper.Map<SMChannelDto>(channel);
    }

    public async Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> query = GetQuery(parameters);
        return await ToggleSMChannelsVisibleById([.. query.Select(a => a.Id)], cancellationToken);
    }

    public async Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> smChannels = GetQuery(parameters);
        return await AutoSetEPGs(smChannels, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> smChannels = GetQuery(a => ids.Contains(a.Id));
        return await AutoSetEPGs(smChannels, cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<FieldData>> AutoSetEPGs(IQueryable<SMChannel> smChannels, CancellationToken cancellationToken)
    {
        List<StationChannelName> stationChannelNames = await schedulesDirectDataService.GetStationChannelNames();
        stationChannelNames = stationChannelNames.OrderBy(a => a.Channel).ToList();

        List<string> tomatch = stationChannelNames.Select(a => a.DisplayName).Distinct().ToList();
        string tomatchString = string.Join(',', tomatch);

        List<FieldData> fds = [];

        List<SMChannel> smChannelList = smChannels.ToList();


        foreach (SMChannel smChannel in smChannelList)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                if (fds.Count != 0)
                {
                    await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
                return fds;
            }

            try
            {
                var scoredMatches = stationChannelNames.Select(p => new
                {
                    Channel = p,
                    Score = AutoEPGMatch.GetMatchingScore(smChannel.Name, p.Channel)
                })
                .Where(x => x.Score > 0)
                .OrderByDescending(x => x.Score)
                .ToList();

                if (scoredMatches.Count == 0)
                {
                    scoredMatches = stationChannelNames
                        .Select(p => new
                        {
                            Channel = p,
                            Score = AutoEPGMatch.GetMatchingScore(smChannel.Name, p.DisplayName)
                        })
                        .Where(x => x.Score > 0)
                        .OrderByDescending(x => x.Score)
                        .ToList();
                }

                if (scoredMatches.Count != 0)
                {
                    smChannel.EPGId = scoredMatches[0].Channel.Channel;

                    fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "EPGId", smChannel.EPGId));

                    if (Settings.VideoStreamAlwaysUseEPGLogo)
                    {
                        if (SetVideoStreamLogoFromEPG(smChannel))
                        {
                            fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
                        }
                    }

                    Update(smChannel);
                    //RepositoryContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("An error occurred while processing channel {ChannelName}: {ErrorMessage}", smChannel.Name, ex.Message);
            }
        }


        if (fds.Count != 0)
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return fds;
    }

    private bool SetVideoStreamLogoFromEPG(SMChannel smChannel)
    {
        MxfService? service = schedulesDirectDataService.GetService(smChannel.EPGId);

        if (service is null || !service.extras.TryGetValue("logo", out dynamic? value))
        {
            return false;
        }
        StationImage logo = value;

        if (logo.Url != null)
        {
            smChannel.Logo = logo.Url;
        }
        return true;
    }

    public async Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> smChannels = GetQuery(a => ids.Contains(a.Id));
        return await SetSMChannelsLogoFromEPG(smChannels, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> query = GetQuery(parameters);
        return await SetSMChannelsLogoFromEPG(query, cancellationToken);
    }

    private async Task<List<FieldData>> SetSMChannelsLogoFromEPG(IQueryable<SMChannel> smChannels, CancellationToken cancellationToken)
    {
        List<FieldData> fds = [];
        foreach (SMChannel? smChannel in smChannels.Where(a => !string.IsNullOrEmpty(a.EPGId)))
        {
            if (SetVideoStreamLogoFromEPG(smChannel))
            {
                fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
            }
        }

        if (fds.Count != 0)
        {
            await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return fds;
    }

    public async Task<APIResponse> SetSMChannelProxy(int sMChannelId, int streamingProxy)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.StreamingProxyType = (StreamingProxyTypes)Enum.ToObject(typeof(StreamingProxyTypes), streamingProxy);
        Update(channel);
        await SaveChangesAsync();

        return APIResponse.Success;
    }
}
