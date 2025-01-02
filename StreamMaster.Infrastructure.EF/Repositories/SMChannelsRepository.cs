using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text.Json;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using StreamMaster.Application.Common;
using StreamMaster.Application.Interfaces;
using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;
using StreamMaster.Domain.Exceptions;
using StreamMaster.Domain.Filtering;
using StreamMaster.Domain.Helpers;
using StreamMaster.Domain.XmltvXml;
using StreamMaster.EPG;
using StreamMaster.Infrastructure.EF.PGSQL;
using StreamMaster.SchedulesDirect.Domain.Interfaces;
using StreamMaster.Streams.Domain.Interfaces;
namespace StreamMaster.Infrastructure.EF.Repositories;

public class SMChannelsRepository(ILogger<SMChannelsRepository> intLogger, IEpgMatcher epgMatcher, ILogoService logoService, ICacheManager cacheManager, IImageDownloadService imageDownloadService, IImageDownloadQueue imageDownloadQueue, IServiceProvider serviceProvider, IRepositoryWrapper repository, IRepositoryContext repositoryContext, IMapper mapper, IOptionsMonitor<Setting> settings, IOptionsMonitor<CommandProfileDict> intProfileSettings, ISchedulesDirectDataService schedulesDirectDataService)
    : RepositoryBase<SMChannel>(repositoryContext, intLogger), ISMChannelsRepository
{
    private int currentChannelNumber;
    private ConcurrentHashSet<int> existingNumbers = [];
    private ConcurrentHashSet<int> usedNumbers = [];

    public async Task<List<FieldData>> AutoSetEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> smChannels = GetQuery(a => ids.Contains(a.Id));
        return await AutoSetEPGs(smChannels, false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> AutoSetEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> results = await GetPagedSMChannelsQueryableAsync(parameters);
        return await AutoSetEPGs(results, false, cancellationToken).ConfigureAwait(false);
    }
    public async Task<List<FieldData>> AutoSetEPGs(IQueryable<SMChannel> smChannels, bool skipSave, CancellationToken cancellationToken)
    {
        // Apply filtering in the database (deferred execution until ToListAsync is called)
        IQueryable<SMChannel> filteredChannels = smChannels.Where(channel => channel.EPGId != "Dummy");

        // Materialize filtered channels for parallel processing
        List<SMChannel> smChannelsList = await filteredChannels.ToListAsync(cancellationToken).ConfigureAwait(false);

        // Cache station channel names into a list for reuse
        List<StationChannelName> stationChannelNames = [.. schedulesDirectDataService.GetStationChannelNames()];
        if (stationChannelNames.Count == 0)
        {
            return [];
        }

        // Prepare data structures for concurrent operations
        HashSet<string> toMatch = [.. stationChannelNames.Select(a => a.DisplayName)];
        string toMatchString = string.Join(',', toMatch);
        ConcurrentBag<FieldData> fieldDataBag = [];
        ConcurrentQueue<SMChannel> entitiesToUpdate = new();

        // Parallel processing of SMChannels
        await Parallel.ForEachAsync(smChannelsList, cancellationToken, async (smChannel, ct) =>
        {
            if (ct.IsCancellationRequested)
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

            StationChannelName? match = await epgMatcher.MatchAsync(smChannel, ct).ConfigureAwait(false);
            if (match is not null)
            {
                smChannel.EPGId = match.Id;

                entitiesToUpdate.Enqueue(smChannel);

                fieldDataBag.Add(new FieldData(SMChannel.APIName, smChannel.Id, "EPGId", smChannel.EPGId));
                if (settings.CurrentValue.VideoStreamAlwaysUseEPGLogo && SetVideoStreamLogoFromEPG(smChannel))
                {
                    fieldDataBag.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
                }
            }
        }).ConfigureAwait(false);

        if (entitiesToUpdate.IsEmpty)
        {
            return [];
        }

        // Batch update database
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            PGSQLRepositoryContext context = scope.ServiceProvider.GetRequiredService<PGSQLRepositoryContext>();

            List<SMChannel> updatedEntities = [.. entitiesToUpdate];

            foreach (SMChannel smChannel in updatedEntities)
            {
                context.SMChannels.Attach(smChannel);
                context.Entry(smChannel).State = EntityState.Modified;
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // Save additional changes if not skipped
        if (!skipSave && !fieldDataBag.IsEmpty)
        {
            _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        return [.. fieldDataBag];
    }

    //public async Task<List<FieldData>> AutoSetEPGs(List<SMChannel> smChannels, bool skipSave, CancellationToken cancellationToken)
    //{
    //    IEnumerable<StationChannelName> stationChannelNames = schedulesDirectDataService.GetStationChannelNames();// [.. schedulesDirectDataService.GetStationChannelNames().OrderBy(a => a.Channel)];

    //    if (!stationChannelNames.Any())
    //    {
    //        return [];
    //    }

    //    HashSet<string> toMatch = [.. stationChannelNames.Select(a => a.DisplayName).Distinct()];
    //    string toMatchString = string.Join(',', toMatch);

    //    ConcurrentBag<FieldData> fds = []; // Use ConcurrentBag for thread-safe operations

    //    List<StationChannelName> stationChannelList = [.. stationChannelNames];

    //    ConcurrentQueue<SMChannel> entitiesToUpdate = new();

    //    await Parallel.ForEachAsync(smChannels, cancellationToken, async (smChannel, ct) =>
    //    {
    //        if (ct.IsCancellationRequested || smChannel.EPGId == "Dummy")
    //        {
    //            return;
    //        }

    //        string stationId = smChannel.EPGId;
    //        int epgNumber = EPGHelper.DummyId;

    //        if (EPGHelper.IsValidEPGId(smChannel.EPGId))
    //        {
    //            (epgNumber, stationId) = smChannel.EPGId.ExtractEPGNumberAndStationId();
    //        }

    //        if (epgNumber < EPGHelper.DummyId)
    //        {
    //            return;
    //        }

    //        StationChannelName? test = await epgMatcher.MatchAsync(smChannel, ct).ConfigureAwait(false);
    //        if (test is not null)
    //        {
    //            smChannel.EPGId = test.Id;

    //            entitiesToUpdate.Enqueue(smChannel);

    //            fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "EPGId", smChannel.EPGId));
    //            if (settings.CurrentValue.VideoStreamAlwaysUseEPGLogo && SetVideoStreamLogoFromEPG(smChannel))
    //            {
    //                fds.Add(new FieldData(SMChannel.APIName, smChannel.Id, "Logo", smChannel.Logo));
    //            }
    //        }
    //    }).ConfigureAwait(false);

    //    if (entitiesToUpdate.IsEmpty)
    //    {
    //        return [];
    //    }

    //    // After this line, all channels have been processed.
    //    using (IServiceScope scope = serviceProvider.CreateScope())
    //    {
    //        PGSQLRepositoryContext context = scope.ServiceProvider.GetRequiredService<PGSQLRepositoryContext>();

    //        foreach (SMChannel smChannel in entitiesToUpdate)
    //        {
    //            context.SMChannels.Attach(smChannel);
    //            context.Entry(smChannel).State = EntityState.Modified;
    //        }

    //        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    //    }

    //    if (!skipSave && !fds.IsEmpty)
    //    {
    //        _ = await RepositoryContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    //    }

    //    return [.. fds];
    //}

    public async Task<IdIntResultWithResponse> AutoSetSMChannelNumbersFromParameters(int streamGroupId, QueryStringParameters Parameters, int? StartingNumber, bool? OverwriteExisting)
    {
        IQueryable<SMChannel> query = await GetPagedSMChannelsQueryableAsync(Parameters);
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
                SMStream = link.SMStream!,
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
                List<string> batch = [.. streamIds.Skip(i).Take(settings.CurrentValue.DBBatchSize)];

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

                    if (!string.IsNullOrEmpty(smStream.Logo))
                    {
                        LogoInfo logoInfo = new(smStream);
                        imageDownloadQueue.EnqueueLogo(logoInfo);
                    }

                    addedSMChannels.Add(smChannel);
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
        IQueryable<SMChannel> queryableChannels = await GetPagedSMChannelsQueryableAsync(parameters).ConfigureAwait(false);
        IQueryable<SMChannel> toDelete = queryableChannels.Where(a => a.SMChannelType == SMChannelTypeEnum.Regular && !a.IsSystem);
        return await DeleteSMChannelsAsync(toDelete).ConfigureAwait(false);
    }

    public async Task<PagedResponse<SMChannelDto>> GetPagedSMChannels(QueryStringParameters parameters)
    {
        IQueryable<SMChannel> query = await GetPagedSMChannelsQueryableAsync(parameters);

        return await query.GetPagedResponseAsync<SMChannel, SMChannelDto>(parameters.PageNumber, parameters.PageSize, mapper)
                              .ConfigureAwait(false);
    }

    public async Task<IQueryable<SMChannel>> GetPagedSMChannelsQueryableAsync(QueryStringParameters parameters, bool? tracking = false)
    {
        IQueryable<SMChannel> query = GetQuery(parameters, tracking == true)
            .Include(a => a.SMStreams)
            .ThenInclude(a => a.SMStream)
            .Include(a => a.StreamGroups);

        int defaultSGID;
        if (cacheManager.DefaultSG == null)
        {
            IServiceScope scope = serviceProvider.CreateScope();
            IStreamGroupService streamGroupService = scope.ServiceProvider.GetRequiredService<IStreamGroupService>();
            defaultSGID = await streamGroupService.GetDefaultSGIdAsync();
        }
        else
        {
            defaultSGID = cacheManager.DefaultSG.Id;
        }

        if (!string.IsNullOrEmpty(parameters.JSONFiltersString))
        {
            List<DataTableFilterMetaData>? filters = JsonSerializer.Deserialize<List<DataTableFilterMetaData>>(parameters.JSONFiltersString);
            if (filters is null || filters.Count == 0)
            {
                return query;
            }

            // Filter for "inSG" 
            DataTableFilterMetaData? inSGFilter = filters.FirstOrDefault(a => a.MatchMode == "inSG");
            if (inSGFilter?.Value is JsonElement inSGJsonElement)
            {
                string? test = GetElementValue(inSGJsonElement);

                if (!string.IsNullOrEmpty(test) && int.TryParse(test, out int sgNum))
                {
                    query = query.Where(a => repository.StreamGroupSMChannelLink
                        .GetQueryNoTracking
                        .Where(link => link.StreamGroupId == sgNum)
                        .Select(link => link.SMChannelId)
                        .Contains(a.Id));
                }
            }

            // Filter for "notInSG"
            DataTableFilterMetaData? notInSGFilter = filters.FirstOrDefault(a => a.MatchMode == "notInSG");
            if (notInSGFilter?.Value is JsonElement jsonElement)
            {
                string? test = GetElementValue(jsonElement);

                if (!string.IsNullOrEmpty(test) && int.TryParse(test, out int sgNum))
                {
                    query = query.Where(a => !repository.StreamGroupSMChannelLink
                        .GetQueryNoTracking
                        .Where(link => link.StreamGroupId == sgNum)
                        .Select(link => link.SMChannelId)
                        .Contains(a.Id));
                }
            }
        }
        return query;
    }

    private static string? GetElementValue(JsonElement jsonElement)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => jsonElement.GetString(),
            JsonValueKind.Number => jsonElement.TryGetInt32(out int intValue) ? intValue.ToString() : jsonElement.GetDouble().ToString(),
            _ => jsonElement.ToString(),
        };
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
        if (
            string.IsNullOrWhiteSpace(logo) ||
                !(
                logo.StartsWithIgnoreCase("http") ||
                 logo.StartsWithIgnoreCase("/images/") ||
                logo.StartsWithIgnoreCase("data:") ||
                logo.StartsWithIgnoreCase("/api/files/cu/")
                )
             )
        {
            return APIResponse.ErrorWithMessage("Invalid logo URL");
        }

        SMChannel? channel = GetSMChannel(SMChannelId);
        if (channel == null)
        {
            return APIResponse.ErrorWithMessage($"Channel {SMChannelId} doesn't exist");
        }

        if (ImageConverter.IsData(logo))
        {
            LogoInfo nl = new(logo);
            logo = logoService.AddCustomLogo(channel.Name, nl.FileName);
            await imageDownloadService.DownloadImageAsync(nl, CancellationToken.None);
        }
        else
        {
            if (!logo.IsRedirect() && logo.StartsWithIgnoreCase("http"))
            {
                LogoInfo nl = new(logo);
                await imageDownloadService.DownloadImageAsync(nl, CancellationToken.None);
            }
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
        IQueryable<SMChannel> toUpdate = await GetPagedSMChannelsQueryableAsync(parameters, tracking: true);
        return await SetSMChannelsCommandProfileName(toUpdate, CommandProfileName);
    }

    public async Task<APIResponse> SetSMChannelsGroup(List<int> sMChannelIds, string GroupName)
    {
        IQueryable<SMChannel> toUpdate = GetQuery(tracking: true).Where(a => sMChannelIds.Contains(a.Id));
        return await SetSMChannelsGroup(toUpdate, GroupName);
    }

    public async Task<APIResponse> SetSMChannelsGroupFromParameters(QueryStringParameters parameters, string GroupName)
    {
        IQueryable<SMChannel> toUpdate = await GetPagedSMChannelsQueryableAsync(parameters, tracking: true);
        return await SetSMChannelsGroup(toUpdate, GroupName);
    }

    public async Task<List<FieldData>> SetSMChannelsLogoFromEPGFromIds(List<int> ids, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> smChannels = GetQuery(a => ids.Contains(a.Id));
        return await SetSMChannelsLogoFromEPG(smChannels, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<FieldData>> SetSMChannelsLogoFromEPGFromParameters(QueryStringParameters parameters, CancellationToken cancellationToken)
    {
        IQueryable<SMChannel> query = await GetPagedSMChannelsQueryableAsync(parameters);
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
            List<SMChannel> batch = [.. addedSMChannels.Skip(i).Take(settings.CurrentValue.DBBatchSize)];

            if (settings.CurrentValue.AutoSetEPG)
            {
                Stopwatch AutoSetEPGStopwatch = Stopwatch.StartNew();
                _ = await AutoSetEPGs(batch.AsQueryable(), true, CancellationToken.None).ConfigureAwait(false);
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

        //LogoInfo  nl = new(smStream);

        //string logo = logoService.GetLogoUrl2(smStream.ChannelLogo, ftype);
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
            Logo = smStream.Logo,// nl.SMLogoUrl,
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
        const int batchSize = 1000; // Adjust batch size based on your system's capacity
        List<int> deletedIds = [];

        try
        {
            // Get all channel IDs as a list
            List<int> channelIds = await channels.Select(a => a.Id).ToListAsync().ConfigureAwait(false);

            if (channelIds.Count == 0)
            {
                return deletedIds;
            }

            // Process in batches
            foreach (IEnumerable<int> batch in channelIds.Batch(batchSize))
            {
                string channelIdsString = string.Join(",", batch);

                // Call the PostgreSQL function for the current batch
                await RepositoryContext.ExecuteSqlRawAsync(
                    $"SELECT * FROM public.delete_sm_channels(ARRAY[{channelIdsString}]::INTEGER[])").ConfigureAwait(false);

                deletedIds.AddRange(batch);
            }

            return deletedIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting SMChannels");
            return [];
        }
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

            //case M3UField.Url:
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
        StationChannelName? match = cacheManager.StationChannelNames
     .SelectMany(kvp => kvp.Value)
     .FirstOrDefault(stationchannel => stationchannel.Id == smChannel.EPGId || stationchannel.Channel == smChannel.EPGId);

        if (match is not null && match.Logo != string.Empty && smChannel.Logo != match.Logo)
        {
            smChannel.Logo = match.Logo;
            return true;
        }

        return false;
    }
}