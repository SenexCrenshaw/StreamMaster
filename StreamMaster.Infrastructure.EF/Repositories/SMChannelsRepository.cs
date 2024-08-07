using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Exceptions;
using StreamMaster.Domain.Filtering;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Helpers;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;

using System.Linq.Expressions;
using System.Text.Json;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelsRepository(ILogger<SMChannelsRepository> intLogger, IServiceProvider serviceProvider, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> intSettings, IOptionsMonitor<CommandProfileDict> intProfileSettings, ISchedulesDirectDataService schedulesDirectDataService)
    : RepositoryBase<SMChannel>(repositoryContext, intLogger), ISMChannelsRepository
{
    private ConcurrentHashSet<int> existingNumbers = [];
    private ConcurrentHashSet<int> usedNumbers = [];
    private int currentChannelNumber;

    public List<SMChannelDto> GetSMChannels()
    {
        return [.. GetQuery().Include(a => a.SMStreams).ThenInclude(a => a.SMStream).ProjectTo<SMChannelDto>(mapper.ConfigurationProvider)];
    }

    public async Task<APIResponse> SetSMChannelsCommandProfileName(List<int> sMChannelIds, string CommandProfileName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(tracking: true).Where(a => sMChannelIds.Contains(a.Id));
        return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    }

    public async Task<APIResponse> SetSMChannelsCommandProfileNameFromParameters(QueryStringParameters parameters, string CommandProfileName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(parameters, tracking: true);
        return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    }

    private async Task<APIResponse> SetSMChannelsCommandProfileName(IQueryable<SMChannel> query, string CommandProfileName)
    {
        //var profile = intProfileSettings.CurrentValue.GetProfile(CommandProfileName);
        if (!intProfileSettings.CurrentValue.HasProfile(CommandProfileName))
        {
            return APIResponse.ErrorWithMessage($"CommandProfileName '{CommandProfileName}' not found");
        }
        List<SMChannel> toUpdate = await query.ToListAsync();

        foreach (SMChannel smChannl in toUpdate)
        {
            smChannl.CommandProfileName = CommandProfileName;
        }

        BulkUpdate(toUpdate);
        _ = RepositoryContext.SaveChanges();
        return APIResponse.Success;
    }

    public async Task<APIResponse> SetSMChannelCommandProfileName(int sMChannelId, string CommandProfileName)
    {
        SMChannel? channel = GetSMChannel(sMChannelId);
        if (channel == null)
        {
            return APIResponse.NotFound;
        }

        channel.CommandProfileName = CommandProfileName;
        Update(channel);
        _ = await SaveChangesAsync();

        return APIResponse.Success;
    }

    public PagedResponse<SMChannelDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMChannelDto>(Count());
    }

    public async Task ChangeGroupName(string oldGroupName, string newGroupName)
    {
        string sql = $"UPDATE public.\"SMChannels\" SET \"Group\"='{newGroupName}' WHERE \"Group\"={oldGroupName};";
        _ = await RepositoryContext.ExecuteSqlRawAsyncEntities(sql);
    }
    public IQueryable<SMChannel> GetPagedSMChannelsQueryable(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> query = GetQuery(parameters).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups);

        if (!string.IsNullOrEmpty(parameters.JSONFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(parameters.JSONFiltersString);
            if (filters?.Any(a => a.MatchMode == "inSG") == true)
            {
                DataTableFilterMetaData? inSGFilter = filters.Find(a => a.MatchMode == "inSG");
                if (inSGFilter?.Value != null)
                {
                    try
                    {
                        string? streamGroupIdString = inSGFilter.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(streamGroupIdString))
                        {
                            int streamGroupId = Convert.ToInt32(streamGroupIdString);
                            List<int> linkIds = [.. repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId)];
                            query = query.Where(a => linkIds.Contains(a.Id));
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Object value is outside the range of an Int32. Object: {Object}", inSGFilter.Value);
                    }
                }
            }

            if (filters?.Any(a => a.MatchMode == "notInSG") == true)
            {
                DataTableFilterMetaData? notInSGFilter = filters.Find(a => a.MatchMode == "notInSG");
                if (notInSGFilter?.Value != null)
                {
                    try
                    {
                        string? streamGroupIdString = notInSGFilter.Value.ToString();
                        if (!string.IsNullOrWhiteSpace(streamGroupIdString))
                        {
                            int streamGroupId = Convert.ToInt32(streamGroupIdString);
                            List<int> linkIds = [.. repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId)];
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
        return query;
    }
    public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> query = GetPagedSMChannelsQueryable(parameters);

        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                              .ConfigureAwait(false);
    }

    public async Task CreateSMChannel(SMChannel smChannel)
    {
        Create(smChannel);
        _ = await SaveChangesAsync();
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
            _ = await SaveChangesAsync();
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
        IQueryable<SMChannel> toDelete = GetQuery(parameters).Where(a => a.SMChannelType == SMChannelTypeEnum.Regular && !a.IsSystem);
        return await DeleteSMChannelsAsync(toDelete).ConfigureAwait(false);
    }

    public SMChannel? GetSMChannel(int smchannelId)
    {
        return FirstOrDefault(a => a.Id == smchannelId, tracking: false);
    }

    private SMChannel? CreateSMChannelFromStreamNoLink(string streamId, int? M3UFileId = null)
    {
        SMStream? smStream = repository.SMStream.GetSMStreamById(streamId) ?? throw new APIException($"Stream with Id {streamId} is not found");
        if (smStream == null)
        {
            return null;
        }

        SMChannel smChannel = new()
        {
            ChannelNumber = smStream.ChannelNumber,
            Group = smStream.Group,
            Name = smStream.Name,
            Logo = smStream.Logo,
            EPGId = smStream.EPGID,
            StationId = smStream.StationId,
            M3UFileId = M3UFileId ?? smStream.M3UFileId,
            BaseStreamID = smStream.Id,
            IsSystem = smStream.IsSystem,
            CommandProfileName = "Default",
        };
        Create(smChannel);

        return smChannel;
    }

    private async Task<SMChannel?> CreateSMChannelFromStream(string streamId, int? M3UFileId = null, bool? forced = false)
    {
        SMStream? smStream = repository.SMStream.GetSMStreamById(streamId) ?? throw new APIException($"Stream with Id {streamId} is not found");
        if (smStream == null)//|| (forced == false))//&& smStream.IsCustomStream))
        {
            return null;
        }

        SMChannel smChannel = new()
        {
            ChannelNumber = smStream.ChannelNumber,
            Group = smStream.Group,
            Name = smStream.Name,
            Logo = smStream.Logo,
            EPGId = smStream.EPGID,
            StationId = smStream.StationId,
            M3UFileId = M3UFileId ?? smStream.M3UFileId,
            BaseStreamID = smStream.Id,
            IsSystem = smStream.IsSystem,
            CommandProfileName = "Default",
        };
        //Create(smChannel);

        await CreateSMChannel(smChannel);
        //await SaveChangesAsync();
        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel, smStream, null);
        //if (StreamGroup)
        //{
        //    await sender.Send(new AddSMChannelToStreamGroupRequest(StreamGroupId.Value, smChannel.Id));
        //}
        //if (Settings.AutoSetEPG)
        //{
        //    List<FieldData> fds = await AutoSetEPGs(GetQuery(a => a.Id == smChannel.Id), CancellationToken.None);
        //    if (fds.Count != 0)
        //    {
        //        FieldData test = fds.First(a => a.Id == smChannel.Id.ToString() && a.Field == "EPGId");
        //        if (test.Value is string value && !string.IsNullOrEmpty(value))
        //        {
        //            smChannel.EPGId = value;
        //            //Update(smChannel);
        //            //await SaveChangesAsync();
        //        }
        //    }
        //    //RepositoryContext.SaveChanges();
        //}
        //RepositoryContext.SaveChanges();
        return smChannel;
    }

    public async Task<APIResponse> DeleteSMChannels(List<int> smchannelIds)
    {
        IQueryable<SMChannel> toDelete = GetQuery(true).Where(a => smchannelIds.Contains(a.Id) && !a.IsSystem);
        if (!toDelete.Any())
        {
            return APIResponse.NotFound;
        }

        _ = await DeleteSMChannelsAsync(toDelete);

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
            List<SMChannel> a = [.. channels];
            List<int> ret = [.. a.Select(a => a.Id)];
            IQueryable<SMChannelStreamLink> linksToDelete = repository.SMChannelStreamLink.GetQuery(true).Where(a => ret.Contains(a.SMChannelId));
            await repository.SMChannelStreamLink.DeleteSMChannelStreamLinks(linksToDelete);
            _ = await SaveChangesAsync();
            BulkDelete(a);
            _ = await SaveChangesAsync();
            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannels");
        }
        return [];
    }

    public async Task<IdIntResultWithResponse> AutoSetSMChannelNumbersRequest(int StreamGroupId, List<int> SMChannelIds, int? StartingNumber, bool? OverwriteExisting)
    {
        IQueryable<SMChannel> query = GetQuery().Where(a => SMChannelIds.Contains(a.Id));
        return await AutoSetSMChannelNumbers(query, StartingNumber ?? 1, OverwriteExisting ?? true);
    }

    public async Task<IdIntResultWithResponse> AutoSetSMChannelNumbersFromParameters(int streamGroupId, QueryStringParameters Parameters, int? StartingNumber, bool? OverwriteExisting)
    {
        IQueryable<SMChannel> query = GetQuery(Parameters);
        return await AutoSetSMChannelNumbers(query, StartingNumber ?? 1, OverwriteExisting ?? true);
    }

    private async Task<IdIntResultWithResponse> AutoSetSMChannelNumbers(IQueryable<SMChannel> smChannels, int StartingNumber, bool OverwriteExisting)
    {
        if (!smChannels.Any())
        {
            return [];
        }

        existingNumbers = [];
        usedNumbers = [];
        currentChannelNumber = StartingNumber - 1;

        IdIntResultWithResponse ret = [];

        if (!OverwriteExisting)
        {
            foreach (int channelNumber in smChannels.Select(a => a.ChannelNumber).Distinct())
            {
                _ = existingNumbers.Add(channelNumber);
            }
        }
        List<SMChannel> smChannelsList = await smChannels.ToListAsync();

        foreach (SMChannel smChannel in smChannelsList)
        {
            int channelNumber = GetNextChannelNumber(smChannel.ChannelNumber, OverwriteExisting);
            smChannel.ChannelNumber = channelNumber;
            _ = RepositoryContext.SMChannels.Update(smChannel);
            ret.Add(new IdIntResult { Id = smChannel.Id, Result = smChannel });
        }

        _ = await RepositoryContext.SaveChangesAsync();

        //StreamGroup? streamGroup = await GetQuery(true).FirstOrDefaultAsync(a => a.Id == streamGroupId);

        //if (streamGroup == null)
        //{
        //    ret.APIResponse = APIResponse.ErrorWithMessage("Stream CommandProfileName not found");
        //    return ret;
        //}

        //StreamGroupProfile sgProfile = RepositoryContext.StreamGroupProfiles.FirstOrDefault(a => a.StreamGroupId == streamGroupId && a.OutputProfileName == "Default") ?? new StreamGroupProfile();

        //(List<VideoStreamConfig> videoStreamConfigs, OutputProfile outputProfile) = await sender.Send(new GetStreamGroupVideoConfigs(streamGroupId, sgProfile.Id));

        //if (string.IsNullOrEmpty(Parameters.JSONFiltersString))
        //{
        //    ret.APIResponse = APIResponse.ErrorWithMessage("JSONFiltersString null");
        //    return ret;
        //}

        //List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(Parameters.JSONFiltersString);
        //IQueryable<SMChannel> channels = FilterHelper<SMChannel>.ApplyFiltersAndSort(streamGroup.SMChannels.Select(a => a.SMChannel).AsQueryable(), filters, Parameters.OrderBy, true);

        //foreach (SMChannel channel in smChannels)
        //{
        //    channel.ChannelNumber = validconfig.ChannelNumber;

        //    RepositoryContext.SMChannels.Update(channel);
        //    ret.Add(new IdIntResult { Id = channel.Id, Result = channel });
        //}

        //await RepositoryContext.SaveChangesAsync();

        return ret;
    }

    private int GetNextChannelNumber(int channelNumber, bool overwriteExisting)
    {
        if (!overwriteExisting)
        {
            return GetNext();
        }

        if (existingNumbers.Contains(channelNumber))
        {
            if (usedNumbers.Add(channelNumber))
            {
                return channelNumber;
            }
        }

        return GetNext();
    }

    private int GetNext()
    {
        ++currentChannelNumber;
        while (!usedNumbers.Add(currentChannelNumber))
        {
            ++currentChannelNumber;
        }
        return currentChannelNumber;
    }

    public async Task<APIResponse> AddSMStreamToSMChannel(int SMChannelId, string SMStreamId, int? Rank)
    {
        SMChannel? smChannel = GetSMChannel(SMChannelId);
        if (smChannel == null || repository.SMStream.GetSMStream(SMStreamId) == null)
        {
            throw new APIException($"Channel with Id {SMChannelId} or stream with Id {SMStreamId} not found");
        }

        await repository.SMChannelStreamLink.CreateSMChannelStreamLink(smChannel.Id, SMStreamId, Rank);
        _ = await SaveChangesAsync();

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
            return APIResponse.ErrorWithMessage($"Channel {SMChannelId} doesn't exist");
        }

        channel.Logo = logo;
        Update(channel);
        _ = await SaveChangesAsync();

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
        _ = await SaveChangesAsync();

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
        _ = await SaveChangesAsync();

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
        _ = await SaveChangesAsync();

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
        _ = await SaveChangesAsync();

        return APIResponse.Success;
    }

    public async Task<APIResponse> CloneSMChannel(int sMChannelId, string newName)
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
        _ = await SaveChangesAsync();
        List<SMChannelStreamLink> links = [.. repository.SMChannelStreamLink.GetQuery().Where(a => a.SMChannelId == sMChannelId)];

        foreach (SMChannelStreamLink? link in links)
        {
            SMChannelStreamLink newLink = new()
            {
                SMChannel = newChannel,
                SMChannelId = newChannel.Id,
                SMStream = link.SMStream,
                SMStreamId = link.SMStreamId,
                Rank = link.Rank,
            };

            repository.SMChannelStreamLink.Create(newLink);
        }
        _ = await SaveChangesAsync();

        return APIResponse.Success;
    }

    [LogExecutionTimeAspect]
    public async Task<APIResponse> CreateSMChannelsFromStreams(List<string> streamIds, int? addToStreamGroupId)
    {
        try
        {
            List<SMChannel> addedSMChannels = [];
            Setting settings = intSettings.CurrentValue;
            int batchSize = 500;

            for (int i = 0; i < streamIds.Count; i += batchSize)
            {
                List<string> batch = streamIds.Skip(i).Take(batchSize).ToList();
                foreach (string streamId in batch)
                {
                    SMChannel? smChannel = CreateSMChannelFromStreamNoLink(streamId);

                    if (smChannel is null)
                    {
                        _ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
                        return APIResponse.ErrorWithMessage("Error creating SMChannel from streams");
                    }
                    addedSMChannels.Add(smChannel);
                }

                await SaveChangesAsync();
                await BulkUpdate(addedSMChannels, addToStreamGroupId);

                foreach (SMChannel addedSMChannel in addedSMChannels)
                {
                    await repository.SMChannelStreamLink.CreateSMChannelStreamLink(addedSMChannel, addedSMChannel.BaseStreamID, null);
                }
                await SaveChangesAsync();
                logger.LogInformation("{count} channels have been added.", i + batch.Count);
                addedSMChannels.Clear();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SMChannel from streams");
            return APIResponse.ErrorWithMessage(ex, "Error creating SMChannel from streams");
        }
        _ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        return APIResponse.Success;
    }

    private async Task BulkUpdate(List<SMChannel> addedSMChannels, int? defaultSGId)
    {
        Setting settings = intSettings.CurrentValue;
        int batchSize = 500;

        for (int i = 0; i < addedSMChannels.Count; i += batchSize)
        {
            List<SMChannel> batch = addedSMChannels.Skip(i).Take(batchSize).ToList();

            if (settings.AutoSetEPG)
            {
                _ = await AutoSetEPGs(batch, CancellationToken.None).ConfigureAwait(false);
            }

            if (defaultSGId.HasValue)
            {
                foreach (SMChannel smChannel in batch)
                {
                    _ = await repository.StreamGroupSMChannelLink.AddSMChannelToStreamGroup(defaultSGId.Value, smChannel.Id, true).ConfigureAwait(false);
                }
            }
        }
    }



    public async Task<APIResponse> CreateSMChannelsFromStreamParameters(QueryStringParameters Parameters, int? AddToStreamGroupId)
    {
        IQueryable<SMStream> toCreate = repository.SMStream.GetQuery(Parameters);
        APIResponse ret = await CreateSMChannelsFromStreams([.. toCreate.Select(a => a.Id)], AddToStreamGroupId);
        _ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        return ret;
    }

    public async Task<List<FieldData>> ToggleSMChannelsVisibleById(List<int> ids, CancellationToken cancellationToken)
    {
        List<FieldData> ret = [];
        List<SMChannel> channels = [.. GetQuery(true).Where(a => ids.Contains(a.Id))];

        foreach (SMChannel? channel in channels)
        {
            channel.IsHidden = !channel.IsHidden;
            ret.Add(new FieldData(() => channel.IsHidden));
        }
        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
        _ = await SaveChangesAsync();
        return mapper.Map<SMChannelDto>(channel);
    }

    public async Task<List<FieldData>> ToggleSMChannelVisibleByParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> query = GetQuery(parameters);
        return await ToggleSMChannelsVisibleById([.. query.Select(a => a.Id)], cancellationToken);
    }

    public async Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        List<SMChannel> smChannels = await GetQuery(parameters).ToListAsync(cancellationToken: cancellationToken);
        return await AutoSetEPGs(smChannels, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        List<SMChannel> smChannels = await GetQuery(a => ids.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken);
        return await AutoSetEPGs(smChannels, cancellationToken).ConfigureAwait(false);
    }

    private async Task<List<FieldData>> AutoSetEPGs(List<SMChannel> smChannels, CancellationToken cancellationToken)
    {
        List<StationChannelName> stationChannelNames = schedulesDirectDataService.GetStationChannelNames().ToList();
        stationChannelNames = [.. stationChannelNames.OrderBy(a => a.Channel)];

        List<string> toMatch = stationChannelNames.Select(a => a.DisplayName).Distinct().ToList();
        string toMatchString = string.Join(',', toMatch);

        List<FieldData> fds = [];

        //List<SMChannel> smChannelList = [.. smChannels];

        Setting Settings = intSettings.CurrentValue;
        foreach (SMChannel smChannel in smChannels)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                if (fds.Count != 0)
                {
                    _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
                    scoredMatches = [.. stationChannelNames
                        .Select(p => new
                        {
                            Channel = p,
                            Score = AutoEPGMatch.GetMatchingScore(smChannel.Name, p.DisplayName)
                        })
                        .Where(x => x.Score > 0)
                        .OrderByDescending(x => x.Score)];
                }

                if (scoredMatches.Count > 0)
                {
                    smChannel.EPGId = !scoredMatches[0].Channel.Channel.StartsWith(EPGHelper.SchedulesDirectId.ToString()) && scoredMatches.Count > 1 && scoredMatches[1].Channel.Channel.StartsWith(EPGHelper.SchedulesDirectId.ToString())
                        ? scoredMatches[1].Channel.Channel
                        : scoredMatches[0].Channel.Channel;

                    fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "EPGId", smChannel.EPGId));

                    if (Settings.VideoStreamAlwaysUseEPGLogo)
                    {
                        if (SetVideoStreamLogoFromEPG(smChannel))
                        {
                            fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
                        }
                    }

                    EntityState state = RepositoryContext.SMChannels.Entry(smChannel).State;
                    if (state is EntityState.Unchanged)
                    {
                        Update(smChannel);
                    }
                    //RepositoryContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning("An error occurred while processing channel {ChannelName}: {ErrorMessage}", smChannel.Name, ex.Message);
            }
        }

        //if (fds.Count != 0)
        //{
        //    await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        //}
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
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return fds;
    }

    //public async Task<APIResponse> SetSMChannelCommandProfileName(int sMChannelId, string CommandProfileName)
    //{
    //    SMChannel? channel = GetSMChannel(sMChannelId);
    //    if (channel == null)
    //    {
    //        return APIResponse.NotFound;
    //    }

    //    channel.CommandProfileName = CommandProfileName;
    //    Update(channel);
    //    _ = await SaveChangesAsync();

    //    return APIResponse.Success;
    //}

    public async Task<List<SMChannel>> GetSMChannelsFromStreamGroup(int streamGroupId)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();

        if (streamGroupId == await streamGroupService.GetDefaultSGIdAsync())
        {
            return await GetQuery().ToListAsync();
        }

        IQueryable<SMChannel> channels = RepositoryContext.StreamGroupSMChannelLinks.Where(a => a.StreamGroupId == streamGroupId).Include(a => a.SMChannel).Select(a => a.SMChannel);

        return await channels.ToListAsync();
    }

    public SMChannel? GetSMChannelFromStreamGroup(int smChannelId, int streamGroupId)
    {
        IQueryable<SMChannel> channels = RepositoryContext.StreamGroupSMChannelLinks
            .Where(a => a.StreamGroupId == streamGroupId && a.SMChannelId == smChannelId)
            .Include(a => a.SMChannel)
            .ThenInclude(a => a.SMStreams)
            .ThenInclude(a => a.SMStream)
            .Select(a => a.SMChannel);
        SMChannel? ret = channels.FirstOrDefault();
        return channels.FirstOrDefault();
    }

    public async Task<APIResponse> SetSMChannelsGroup(List<int> sMChannelIds, string GroupName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(tracking: true).Where(a => sMChannelIds.Contains(a.Id));
        return await SetSMChannelsGroup(toUpdate, GroupName);
    }

    public async Task<APIResponse> SetSMChannelsGroupFromParameters(QueryStringParameters parameters, string GroupName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(parameters, tracking: true);
        return await SetSMChannelsGroup(toUpdate, GroupName);
    }
    private async Task<APIResponse> SetSMChannelsGroup(IQueryable<SMChannel> query, string GroupName)
    {
        ChannelGroup? group = await RepositoryContext.ChannelGroups.FirstOrDefaultAsync(a => a.Name == GroupName);
        if (group == null)
        {
            return APIResponse.ErrorWithMessage($"Group '{GroupName}' not found");
        }
        List<SMChannel> toUpdate = await query.ToListAsync();

        foreach (SMChannel smChannl in toUpdate)
        {
            smChannl.Group = group.Name;
        }

        BulkUpdate(toUpdate);
        _ = RepositoryContext.SaveChanges();
        return APIResponse.Success;
    }

    //public async Task<APIResponse> SetSMChannelsCommandProfileName(List<int> sMChannelIds, string CommandProfileName)
    //{
    //    IQueryable<SMChannel> toUpdate = GetQuery(tracking: true).Where(a => sMChannelIds.Contains(a.Id));
    //    return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    //}

    //public async Task<APIResponse> SetSMChannelsCommandProfileNameFromParameters(QueryStringParameters parameters, string CommandProfileName)
    //{
    //    IQueryable<SMChannel> toUpdate = GetQuery(parameters, tracking: true);
    //    return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    //}

    //private async Task<APIResponse> SetSMChannelsCommandProfileName(IQueryable<SMChannel> query, string CommandProfileName)
    //{
    //    if (!intProfileSettings.CurrentValue.CommandProfileDict.ContainsKey(CommandProfileName))
    //    {
    //        return APIResponse.ErrorWithMessage($"CommandProfileName '{CommandProfileName}' not found");
    //    }
    //    List<SMChannel> toUpdate = await query.ToListAsync();

    //    foreach (SMChannel smChannl in toUpdate)
    //    {
    //        smChannl.CommandProfileName = CommandProfileName;
    //    }

    //    BulkUpdate(toUpdate);
    //    _ = RepositoryContext.SaveChanges();
    //    return APIResponse.Success;
    //}
}
