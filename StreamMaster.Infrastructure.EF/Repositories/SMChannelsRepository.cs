using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Filtering;
using StreamMaster.Domain.Helpers;
using StreamMaster.Infrastructure.EF.Helpers;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.SchedulesDirect.Domain.JsonClasses;
using StreamMaster.SchedulesDirect.Domain.Models;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;

namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelsRepository(ILogger<SMChannelsRepository> intLogger, IImageDownloadQueue imageDownloadQueue, IServiceProvider serviceProvider, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> settings, IOptionsMonitor<CommandProfileDict> intProfileSettings, ISchedulesDirectDataService schedulesDirectDataService)
    : RepositoryBase<SMChannel>(repositoryContext, intLogger), ISMChannelsRepository
{
    private int currentChannelNumber;
    private ConcurrentHashSet<int> existingNumbers = [];
    private ConcurrentHashSet<int> usedNumbers = [];

    public async Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        List<SMChannel> smChannels = await GetQuery(a => ids.Contains(a.Id)).ToListAsync(cancellationToken: cancellationToken);
        return await AutoSetEPGs(smChannels, false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        List<SMChannel> smChannels = await GetPagedSMChannelsQueryable(parameters).ToListAsync(cancellationToken: cancellationToken);
        return await AutoSetEPGs(smChannels, false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> AutoSetEPGs(List<SMChannel> smChannels, bool skipSave, CancellationToken cancellationToken)
    {
        List<StationChannelName> stationChannelNames = [.. schedulesDirectDataService.GetStationChannelNames().OrderBy(a => a.Channel)];

        HashSet<string> toMatch = [.. stationChannelNames.Select(a => a.DisplayName).Distinct()];
        string toMatchString = string.Join(',', toMatch);

        ConcurrentBag<FieldData> fds = []; // Use ConcurrentBag for thread-safe operations

        List<StationChannelName> stationChannelList = [.. stationChannelNames];

        List<SMChannel> entitiesToUpdate = [];

        await Task.Run(() =>
        {
            Parallel.ForEach(smChannels, new ParallelOptions { CancellationToken = cancellationToken }, smChannel =>
            {
                if (cancellationToken.IsCancellationRequested || smChannel.EPGId == "Dummy")
                {
                    return;
                }

                string stationId = smChannel.EPGId;
                int epgNumber = EPGHelper.DummyId;

                if (EPGHelper.IsValidEPGId(smChannel.EPGId))
                {
                    (epgNumber, stationId) = smChannel.EPGId.ExtractEPGNumberAndStationId();
                }

                if (epgNumber < EPGHelper.DummyId)
                {
                    return;
                }

                try
                {
                    var scoredMatches = stationChannelNames
                        .Select(p => new
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
                        var bestMatch = scoredMatches[0];
                        if (!bestMatch.Channel.Channel.StartsWith(EPGHelper.SchedulesDirectId.ToString()) && scoredMatches.Count > 1 && scoredMatches[1].Channel.Channel.StartsWith(EPGHelper.SchedulesDirectId.ToString()))
                        {
                            bestMatch = scoredMatches[1];
                        }

                        if (smChannel.EPGId != bestMatch.Channel.Channel)
                        {
                            smChannel.EPGId = bestMatch.Channel.Channel;

                            lock (entitiesToUpdate)
                            {
                                entitiesToUpdate.Add(smChannel);
                            }

                            fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "EPGId", smChannel.EPGId));

                            if (settings.CurrentValue.VideoStreamAlwaysUseEPGLogo)
                            {
                                if (SetVideoStreamLogoFromEPG(smChannel))
                                {
                                    fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning("An error occurred while processing channel {ChannelName}: {ErrorMessage}", smChannel.Name, ex.Message);
                }
            });
        }, cancellationToken).ConfigureAwait(false);

        // Batch update the entities after the parallel processing
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            PGSQLRepositoryContext context = scope.ServiceProvider.GetRequiredService<PGSQLRepositoryContext>();

            foreach (SMChannel smChannel in entitiesToUpdate)
            {
                context.SMChannels.Attach(smChannel);
                context.Entry(smChannel).State = EntityState.Modified;
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        if (!skipSave && !fds.IsEmpty)
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return [.. fds];
    }

    public async Task<IdIntResultWithResponse> AutoSetSMChannelNumbersFromParameters(int streamGroupId, QueryStringParameters Parameters, int? StartingNumber, bool? OverwriteExisting)
    {
        IQueryable<SMChannel> query = GetPagedSMChannelsQueryable(Parameters);
        return await AutoSetSMChannelNumbers(query, StartingNumber ?? 1, OverwriteExisting ?? true);
    }

    public async Task<IdIntResultWithResponse> AutoSetSMChannelNumbersRequest(int StreamGroupId, List<int> SMChannelIds, int? StartingNumber, bool? OverwriteExisting)
    {
        IQueryable<SMChannel> query = GetQuery().Where(a => SMChannelIds.Contains(a.Id));
        return await AutoSetSMChannelNumbers(query, StartingNumber ?? 1, OverwriteExisting ?? true);
    }

    public async Task ChangeGroupName(string oldGroupName, string newGroupName)
    {
        string sql = $"UPDATE public.\"SMChannels\" SET \"Group\"='{newGroupName}' WHERE \"Group\"={oldGroupName};";
        _ = await RepositoryContext.ExecuteSqlRawAsync(sql);
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

    public PagedResponse<SMChannelDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<SMChannelDto>(Count());
    }

    public async Task CreateSMChannel(SMChannel smChannel)
    {
        Create(smChannel);
        _ = await SaveChangesAsync();
    }

    public async Task<APIResponse> CreateSMChannelsFromStreamParameters(QueryStringParameters Parameters, int? AddToStreamGroupId)
    {
        IQueryable<SMStream> toCreate = repository.SMStream.GetQuery(Parameters);
        APIResponse ret = await CreateSMChannelsFromStreams([.. toCreate.Select(a => a.Id)], AddToStreamGroupId);
        _ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        return ret;
    }

    [LogExecutionTimeAspect]
    public async Task<APIResponse> CreateSMChannelsFromStreams(List<string> streamIds, int? addToStreamGroupId)
    {
        Stopwatch stopwatch = Stopwatch.StartNew(); // Start the timer
        try
        {
            // Preload streams into a dictionary for quick lookup

            Dictionary<string, SMStream> smStreams = GetSMStreamsByIds(streamIds).ToDictionary(s => s.Id);

            List<SMChannel> addedSMChannels = [];
            //List<SMChannel> bulkSMChannels = [];

            for (int i = 0; i < streamIds.Count; i += settings.CurrentValue.DBBatchSize)
            {
                Stopwatch batchStopwatch = Stopwatch.StartNew(); // Timer for each batch
                List<string> batch = streamIds.Skip(i).Take(settings.CurrentValue.DBBatchSize).ToList();

                foreach (string streamId in batch)
                {
                    if (!smStreams.TryGetValue(streamId, out SMStream? smStream))
                    {
                        logger.LogError("Stream with Id {streamId} not found.", streamId);
                        continue; // Skip if the stream is not found
                    }

                    SMChannel? smChannel = CreateSMChannelFromStreamNoLink(smStream);

                    if (smChannel is null)
                    {
                        _ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
                        return APIResponse.ErrorWithMessage("Error creating SMChannel from streams");
                    }
                    addedSMChannels.Add(smChannel);
                    NameLogo NameLogo = new(smChannel, SMFileTypes.Logo);
                    imageDownloadQueue.EnqueueNameLogo(NameLogo);
                    //logoService.DownloadAndAdd(NameLogo);
                    //bulkSMChannels.Add(smChannel);
                }

                await SaveChangesAsync().ConfigureAwait(false);

                foreach (SMChannel addedSMChannel in addedSMChannels)
                {
                    repository.SMChannelStreamLink.CreateSMChannelStreamLink(addedSMChannel, addedSMChannel.BaseStreamID, null);
                }

                await SMChannelBulkUpdate(addedSMChannels, addToStreamGroupId).ConfigureAwait(false);

                //await SaveChangesAsync().ConfigureAwait(false);
                logger.LogInformation("{count} channels have been added in {elapsed}ms.", i + batch.Count, batchStopwatch.ElapsedMilliseconds);
                addedSMChannels.Clear();
            }
            await SaveChangesAsync().ConfigureAwait(false);
            Stopwatch bulkStopwatch = Stopwatch.StartNew();
            await SMChannelBulkUpdate(addedSMChannels, addToStreamGroupId).ConfigureAwait(false);
            await SaveChangesAsync().ConfigureAwait(false);
            bulkStopwatch.Stop();
            logger.LogInformation("Bulk update {count} channels in {elapsed}ms.", addedSMChannels.Count, bulkStopwatch.ElapsedMilliseconds);

            logger.LogInformation("Total elapsed time: {elapsed}ms.", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating SMChannel from streams after {elapsed}ms.", stopwatch.ElapsedMilliseconds);
            return APIResponse.ErrorWithMessage(ex, "Error creating SMChannel from streams");
        }
        _ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);

        return APIResponse.Success;
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

    public async Task<List<int>> DeleteSMChannelsFromParameters(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> toDelete = GetPagedSMChannelsQueryable(parameters).Where(a => a.SMChannelType == SMChannelTypeEnum.Regular && !a.IsSystem);
        return await DeleteSMChannelsAsync(toDelete).ConfigureAwait(false);
    }

    public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> query = GetPagedSMChannelsQueryable(parameters);

        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                              .ConfigureAwait(false);
    }

    public IQueryable<SMChannel> GetPagedSMChannelsQueryable(QueryStringParameters parameters, bool? tracking = false)
    {
        IQueryable<SMChannel> query = GetQuery(parameters, tracking == true).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups);

        IServiceScope scope = serviceProvider.CreateScope();
        IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();

        int defaultSGID = streamGroupService.GetDefaultSGIdAsync().Result;

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
                            if (streamGroupId != defaultSGID)
                            {
                                List<int> linkIds = [.. repository.StreamGroupSMChannelLink.GetQuery().Where(a => a.StreamGroupId == streamGroupId).Select(a => a.SMChannelId)];
                                query = query.Where(a => linkIds.Contains(a.Id));
                            }
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

    public override IQueryable<SMChannel> GetQuery(bool tracking = false)
    {
        return tracking
            ? base.GetQuery(tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups)
            : base.GetQuery(tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups).AsNoTracking();
    }

    public override IQueryable<SMChannel> GetQuery(Expression<Func<SMChannel, bool>> expression, bool tracking = false)
    {
        return base.GetQuery(expression, tracking).Include(a => a.SMChannels).ThenInclude(a => a.SMChannel).Include(a => a.SMStreams).ThenInclude(a => a.SMStream).Include(a => a.StreamGroups);
    }

    public SMChannel? GetSMChannel(int smchannelId)
    {
        return GetQuery().FirstOrDefault(a => a.Id == smchannelId);
    }

    public async Task<SMChannel?> GetSMChannelFromStreamGroupAsync(int smChannelId, int streamGroupId)
    {
        StreamGroupSMChannelLink? link = await RepositoryContext.StreamGroupSMChannelLinks.FirstOrDefaultAsync(a => a.StreamGroupId == streamGroupId && a.SMChannelId == smChannelId);
        if (link == null)
        {
            return null;
        }
        SMChannel? smChannel = GetQuery().FirstOrDefault(a => a.Id == smChannelId);

        return smChannel;
    }

    public List<SMChannelDto> GetSMChannels()
    {
        return [.. GetQuery()
            .Include(a => a.SMStreams)
            .ThenInclude(a => a.SMStream)
             .Include(a => a.SMChannels)
            .ThenInclude(a => a.SMChannel)
             .Include(a => a.StreamGroups)
            .ProjectTo<SMChannelDto>(mapper.ConfigurationProvider)];
    }

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

    public IQueryable<SMStream> GetSMStreamsByIds(List<string> streamIds)
    {
        return repository.SMStream.GetQuery().Where(s => streamIds.Contains(s.Id));
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

    public async Task<APIResponse> SetSMChannelsCommandProfileName(List<int> sMChannelIds, string CommandProfileName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(tracking: true).Include(a => a.SMStreams)
            .ThenInclude(a => a.SMStream)
             .Include(a => a.SMChannels)
            .ThenInclude(a => a.SMChannel)
             .Include(a => a.StreamGroups)
             .Where(a => sMChannelIds.Contains(a.Id));
        return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    }

    public async Task<APIResponse> SetSMChannelsCommandProfileNameFromParameters(QueryStringParameters parameters, string CommandProfileName)
    {
        IQueryable<SMChannel> toUpdate = GetPagedSMChannelsQueryable(parameters, tracking: true);
        return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    }

    public async Task<APIResponse> SetSMChannelsGroup(List<int> sMChannelIds, string GroupName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(tracking: true).Where(a => sMChannelIds.Contains(a.Id));
        return await SetSMChannelsGroup(toUpdate, GroupName);
    }

    public async Task<APIResponse> SetSMChannelsGroupFromParameters(QueryStringParameters parameters, string GroupName)
    {
        IQueryable<SMChannel> toUpdate = GetPagedSMChannelsQueryable(parameters, tracking: true);
        return await SetSMChannelsGroup(toUpdate, GroupName);
    }

    public async Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> smChannels = GetQuery(a => ids.Contains(a.Id));
        return await SetSMChannelsLogoFromEPG(smChannels, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> query = GetPagedSMChannelsQueryable(parameters);
        return await SetSMChannelsLogoFromEPG(query, cancellationToken);
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

        return ret;
    }

    private async Task SMChannelBulkUpdate(List<SMChannel> addedSMChannels, int? defaultSGId)
    {
        for (int i = 0; i < addedSMChannels.Count; i += settings.CurrentValue.DBBatchSize)
        {
            Stopwatch batchStopwatch = Stopwatch.StartNew(); // Timer for each batch
            List<SMChannel> batch = addedSMChannels.Skip(i).Take(settings.CurrentValue.DBBatchSize).ToList();

            if (settings.CurrentValue.AutoSetEPG)
            {
                Stopwatch AutoSetEPGStopwatch = Stopwatch.StartNew();
                _ = await AutoSetEPGs(batch, true, CancellationToken.None).ConfigureAwait(false);
                AutoSetEPGStopwatch.Stop();
                logger.LogInformation("AutoSet {count} channels have been processed in {elapsed}ms.", batch.Count, AutoSetEPGStopwatch.ElapsedMilliseconds);
                await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
            }

            if (defaultSGId.HasValue)
            {
                List<int> ids = batch.ConvertAll(a => a.Id);
                Stopwatch linksStopwatch = Stopwatch.StartNew();
                await repository.StreamGroupSMChannelLink.AddSMChannelsToStreamGroupAsync(defaultSGId.Value, ids, skipSave: true);
                linksStopwatch.Stop();
                logger.LogInformation("Add StreamGroup {count} channels have been processed in {elapsed}ms.", batch.Count, linksStopwatch.ElapsedMilliseconds);
                await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
            }

            //_ = await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
            batchStopwatch.Stop();
            logger.LogInformation("BulkUpdate {count} channels have been processed in {elapsed}ms.", batch.Count, batchStopwatch.ElapsedMilliseconds);
        }
    }

    private SMChannel? CreateSMChannelFromStreamNoLink(SMStream smStream)
    {
        if (smStream == null)
        {
            return null;
        }

        string name = GetName(smStream);

        SMChannel smChannel = new()
        {
            BaseStreamID = smStream.Id,
            ChannelId = smStream.ChannelId,
            ChannelName = smStream.ChannelName,
            ChannelNumber = smStream.ChannelNumber,
            CommandProfileName = "Default",
            EPGId = smStream.EPGID,
            Group = smStream.Group,
            IsSystem = smStream.IsSystem,
            Logo = smStream.Logo,
            M3UFileId = smStream.M3UFileId,
            Name = name,
            StationId = smStream.StationId,
            TVGName = smStream.TVGName
        };

        Create(smChannel);

        return smChannel;
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
            await BulkDeleteAsync(a);
            _ = await SaveChangesAsync();
            return ret;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannels");
        }
        return [];
    }

    private string GetName(SMStream smStream)
    {
        M3UField? m3uName = repository.M3UFile.GetQuery().FirstOrDefault(a => a.Id == smStream.M3UFileId)?.M3UName;

        string name = smStream.Name;
        switch (m3uName)
        {
            case M3UField.Name:
                name = smStream.Name;
                break;

            case M3UField.TvgName:
                name = smStream.TVGName;
                break;

            //case M3UField.CUID:
            //    name = smStream.CUID;
            //    break;

            case M3UField.TvgID:
                name = smStream.EPGID;
                break;

            //case M3UField.URL:
            //    name = smStream.Url;
            //    break;

            case M3UField.Group:
                name = smStream.Group;
                break;

            case M3UField.ChannelId:
                name = smStream.ChannelId;
                break;

            case M3UField.ChannelName:
                name = smStream.ChannelId;
                break;

            case M3UField.ChannelNumber:
                name = smStream.ChannelNumber.ToString();
                break;

            default:
                break;
        }

        if (string.IsNullOrEmpty(name))
        {
            name = smStream.Name;
        }
        return name;
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

    private async Task<List<FieldData>> SetSMChannelsLogoFromEPG(IQueryable<SMChannel> smChannels, CancellationToken cancellationToken)
    {
        List<FieldData> fds = [];
        foreach (SMChannel? smChannel in smChannels.Where(a => !string.IsNullOrEmpty(a.EPGId)))
        {
            if (SetVideoStreamLogoFromEPG(smChannel))
            {
                fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
                RepositoryContext.SMChannels.Update(smChannel);
            }
        }

        if (fds.Count != 0)
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
}