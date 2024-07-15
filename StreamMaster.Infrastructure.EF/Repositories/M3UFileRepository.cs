using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.API;
using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Provides methods for performing CRUD operations on M3UFile entities.
/// </summary>
public class M3UFileRepository(ILogger<M3UFileRepository> intLogger, IMessageService messageService, RepositoryWrapper repositoryWrapper, IJobStatusService jobStatusService, IRepositoryContext repositoryContext, IOptionsMonitor<Setting> intSettings, IMapper mapper)
    : RepositoryBase<M3UFile>(repositoryContext, intLogger), IM3UFileRepository
{
    public PagedResponse<M3UFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<M3UFileDto>(Count());
    }

    [LogExecutionTimeAspect]
    public async Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerProcessM3U(M3UFileId);
        if (jobManager.IsRunning)//|| jobManager.IsErrored)
        {
            return null;
        }

        jobManager.Start();
        M3UFile? m3uFile;
        try
        {
            m3uFile = await GetM3UFile(M3UFileId);
            if (m3uFile == null)
            {
                logger.LogCritical("Could parse M3U file M3UFileId: {M3UFileId}", M3UFileId);
                jobManager.SetError();
                return null;
            }

            (List<SMStream>? streams, int streamCount) = await ProcessStreams(m3uFile).ConfigureAwait(false);
            if (streams == null)
            {
                logger.LogCritical("Error while processing M3U file {m3uFile.Name}, bad format", m3uFile.Name);
                jobManager.SetError();
                return null;
            }

            if (!ForceRun && !ShouldUpdate(m3uFile, m3uFile.VODTags))
            {
                jobManager.SetSuccessful();
                return m3uFile;
            }

            await ProcessAndUpdateStreams(m3uFile, streams);
            await UpdateChannelGroups(streams);

            m3uFile.LastUpdated = SMDT.UtcNow;
            UpdateM3UFile(m3uFile);
            await SaveChangesAsync();

            jobManager.SetSuccessful();
            return m3uFile;
        }
        catch (Exception ex)
        {
            jobManager.SetError();
            logger.LogCritical(ex, "Error while processing M3U file");
            return null;
        }
        finally
        {
            //if (m3uFile != null)
            //{
            //    m3uFile.LastUpdated = SMDT.UtcNow;
            //    UpdateM3UFile(m3uFile);
            //    await SaveChangesAsync();
            //}
        }
    }
    private async Task UpdateChannelGroups(List<SMStream> streams)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<string> newGroups = streams.Where(a => a.Group is not null and not "").Select(a => a.Group).Distinct().ToList();
        List<ChannelGroup> channelGroups = await repositoryWrapper.ChannelGroup.GetQuery().ToListAsync();

        CreateNewChannelGroups(newGroups, channelGroups);

        logger.LogInformation("Updating channel groups took {sw.Elapsed.TotalSeconds} seconds", sw.Elapsed.TotalSeconds);
    }

    private APIResponse CreateNewChannelGroups(List<string> newGroups, List<ChannelGroup> existingGroups)
    {
        IEnumerable<string> existingNames = existingGroups.Select(a => a.Name);
        List<string> toCreate = newGroups.Where(a => !existingNames.Contains(a)).ToList();

        return repositoryWrapper.ChannelGroup.CreateChannelGroups(toCreate, true);
    }

    [LogExecutionTimeAspect]
    private async Task ProcessAndUpdateStreams(M3UFile m3uFile, List<SMStream> streams)
    {
        List<string> streamIds = streams.ConvertAll(a => a.Id);
        int removedCount = await RemoveMissingStreams(streamIds, m3uFile.Id);

        (int newStreamCount, int dupStreamCount) = await ProcessStreamsConcurrently(streams, m3uFile);

        m3uFile.LastUpdated = SMDT.UtcNow;
        m3uFile.StreamCount = streams.Count;

        logger.LogInformation("Processed {m3uFile.Name} : {streams.Count} total streams in file, Added {newStreamCount} new streams, removed {streamIds.Count} missing steams and ignored {dupStreamCount} duplicate streams", m3uFile.Name, streams.Count, newStreamCount, removedCount, dupStreamCount);
        await messageService.SendSuccess($"{streams.Count} total streams in file, Added {newStreamCount} \r\nnew streams, removed {removedCount} missing steams and ignored {dupStreamCount} duplicate streams", $"Processed M3U {m3uFile.Name}");

        UpdateM3UFile(m3uFile);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Removes streams that are not present in the provided list from the database, along with their related entities.
    /// </summary>
    /// <param name="m3uFileId">The identifier for the M3U file associated with these streams.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="Exception">Throws an exception if the operation fails.</exception>
    private async Task<int> RemoveMissingStreams(List<string> streamIds, int m3uFileId)
    {
        try
        {
            List<string> toDeleteStreamIds = await repositoryWrapper.SMStream
                .GetQuery(a => a.M3UFileId == m3uFileId && !streamIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            if (toDeleteStreamIds.Count != 0)
            {
                await DeleteRelatedVideoStreamLinks(toDeleteStreamIds);
                await DeleteSMStreams(toDeleteStreamIds);
                return toDeleteStreamIds.Count;
            }
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove missing streams for M3UFileId {M3UFileId}", m3uFileId);
            throw;
        }
    }

    private async Task DeleteRelatedVideoStreamLinks(List<string> streamIds)
    {
        IQueryable<SMChannelStreamLink> toDelete = repositoryWrapper.SMChannelStreamLink.GetQuery(a => streamIds.Contains(a.SMStreamId));
        if (!await toDelete.AnyAsync())
        {
            return;
        }
        await repositoryWrapper.SMChannelStreamLink.BulkDeleteAsync(toDelete).ConfigureAwait(false);
    }

    private async Task DeleteSMStreams(List<string> streamIds)
    {
        if (streamIds.Count == 0)
        {
            return;
        }
        IQueryable<SMStream> toDelete = repositoryWrapper.SMStream.GetQuery(a => streamIds.Contains(a.Id));

        if (!await toDelete.AnyAsync().ConfigureAwait(false))
        {
            return;
        }
        await repositoryWrapper.SMStream.BulkDeleteAsync(toDelete).ConfigureAwait(false);
    }

    [LogExecutionTimeAspect]
    private async Task<(List<SMStream>? streams, int streamCount)> ProcessStreams(M3UFile m3uFile)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<SMStream>? streams = await m3uFile.GetSMStreamsFromM3U(logger).ConfigureAwait(false);

        int streamsCount = 0;
        if (streams != null)
        {
            streamsCount = GetRealSMStreamCount(streams);
            streams = RemoveIgnoredStreams(streams);
            streams = RemoveDuplicates(streams);
        }
        sw.Stop();
        logger.LogInformation("Processing M3U {streamsCount}, streams took {sw.Elapsed.TotalSeconds} seconds", streamsCount, sw.Elapsed.TotalSeconds);
        return (streams, streamsCount);
    }

    private static int GetRealSMStreamCount(List<SMStream> streams)
    {
        List<string> ids = streams.Select(a => a.Id).Distinct().ToList();
        return ids.Count;
    }
    private List<SMStream> RemoveIgnoredStreams(List<SMStream> streams)
    {
        Setting Settings = intSettings.CurrentValue;
        if (Settings.NameRegex.Count == 0)
        {
            return streams;
        }

        foreach (string regex in Settings.NameRegex)
        {
            List<SMStream> toIgnore = ListHelper.GetMatchingProperty(streams, "name", regex);
            logger.LogInformation("Ignoring {toIgnore.Count} streams with regex {regex}", toIgnore.Count, regex);
            _ = streams.RemoveAll(toIgnore.Contains);
        }

        return streams;
    }

    private List<SMStream> RemoveDuplicates(List<SMStream> streams)
    {
        List<SMStream> cleanStreams = streams.GroupBy(s => s.Id)
                  .Select(g => g.First())
                  .ToList();

        logger.LogInformation("Removed {streams.Count - cleanStreams.Count} duplicate streams", streams.Count - cleanStreams.Count);

        return cleanStreams;
    }

    private static bool ShouldUpdate(M3UFile m3uFile, List<string> VODTags)
    {
        if (VODTags.Count > 0)
        {
            return true;
        }

        M3UFile? json = m3uFile.ReadJSON();
        return json is null || m3uFile.LastWrite() >= m3uFile.LastUpdated;
    }

    public void CreateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to create a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }
        Create(m3uFile);
        logger.LogInformation("Created M3UFile with ID: {m3uFile.Id}.", m3uFile.Id);
    }

    /// <inheritdoc/>
    public async Task<M3UFileDto?> DeleteM3UFile(int M3UFileId)
    {
        if (M3UFileId <= 0)
        {
            throw new ArgumentNullException(nameof(M3UFileId));
        }

        M3UFile? m3uFile = await FirstOrDefaultAsync(a => a.Id == M3UFileId).ConfigureAwait(false);
        if (m3uFile == null)
        {
            return null;
        }

        Delete(m3uFile);
        logger.LogInformation("M3UFile with Name {m3uFile.Name} was deleted.", m3uFile.Name);
        return mapper.Map<M3UFileDto>(m3uFile);
    }

    /// <inheritdoc/>
    public async Task<List<M3UFileDto>> GetM3UFiles()
    {
        return await GetQuery().ProjectTo<M3UFileDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
    }

    public async Task<M3UFile?> GetM3UFile(int Id)
    {
        return await FirstOrDefaultAsync(c => c.Id == Id, false).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<M3UFile?> GetM3UFileBySource(string Source)
    {
        M3UFile? m3uFile = await FirstOrDefaultAsync(c => c.Source == Source).ConfigureAwait(false);

        return m3uFile;
    }

    /// <inheritdoc/>
    public async Task<int> GetM3UMaxStreamCount()
    {
        return await GetQuery().SumAsync(a => a.MaxStreamCount).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(QueryStringParameters parameters)
    {
        IQueryable<M3UFile> query = GetQuery(parameters);
        return await query.GetPagedResponseAsync<M3UFile, M3UFileDto>(parameters.PageNumber, parameters.PageSize, mapper)
                          .ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void UpdateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to update a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }
        Update(m3uFile);
        m3uFile.WriteJSON();

        logger.LogInformation("Updated M3UFile with ID: {m3uFile.Id}.", m3uFile.Id);
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetM3UFileNames()
    {
        return await GetQuery()
                     .OrderBy(a => a.Name)
                     .Select(a => a.Name)
                     .ToListAsync()
                     .ConfigureAwait(false);
    }

    public async Task<List<M3UFileDto>> GetM3UFilesNeedUpdating()
    {
        List<M3UFileDto> ret = [];
        List<M3UFileDto> m3uFilesToUpdated = await GetQuery(a => a.AutoUpdate && !string.IsNullOrEmpty(a.Url) && a.HoursToUpdate > 0 && a.LastDownloaded.AddHours(a.HoursToUpdate) < DateTime.Now).ProjectTo<M3UFileDto>(mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false);
        ret.AddRange(m3uFilesToUpdated);
        foreach (M3UFile? m3uFile in GetQuery(a => string.IsNullOrEmpty(a.Url)))
        {
            if (m3uFile.LastWrite() >= m3uFile.LastUpdated)
            {
                ret.Add(mapper.Map<M3UFileDto>(m3uFile));
            }
        }
        return ret;
    }

    public IQueryable<M3UFile> GetM3UFileQuery()
    {
        return GetQuery();
    }

    private async Task<(int newStreamCount, int dupStreamCount)> ProcessStreamsConcurrently(List<SMStream> streams, M3UFile m3uFile)
    {
        //List<SMStream> allStreamURLs = await repositoryWrapper.SMStream.GetQuery().Where(a => a.M3UFileId == m3uFile.Id).ToListAsync().ConfigureAwait(false);
        //List<SMStream> existing = await repositoryWrapper.SMStream.GetQuery().Where(a => a.M3UFileId == m3uFile.Id).ToListAsync().ConfigureAwait(false);
        //List<ChannelGroup> groups = await repositoryWrapper.ChannelGroup.GetQuery().ToListAsync();

        //int newCount = 0;
        int dupTotalCount = 0;
        int processedCount = 0;

        HashSet<string> allLookup = [.. repositoryWrapper.SMStream.GetQuery().Select(a => a.Id)];
        Dictionary<string, SMStream> existingLookup = await repositoryWrapper.SMStream.GetQuery().Where(a => a.M3UFileId == m3uFile.Id).ToDictionaryAsync(a => a.Id, a => a);
        Dictionary<string, ChannelGroup> groupLookup = await repositoryWrapper.ChannelGroup.GetQuery().ToDictionaryAsync(g => g.Name, g => g);
        ConcurrentDictionary<string, bool> processed = new();

        ConcurrentBag<SMStream> toWrite = [];
        ConcurrentBag<SMStream> toUpdate = [];
        ConcurrentDictionary<string, byte> generatedIdsDict = new();

        foreach (SMStream stream in streams)
        {
            generatedIdsDict.TryAdd(stream.ShortSMStreamId, 0);
        }

        _ = Parallel.ForEach(streams.Select((stream, index) => (stream, index)), tuple =>
        {
            SMStream stream = tuple.stream;
            int index = tuple.index;

            if (allLookup.Contains(stream.Id))
            {
                Interlocked.Increment(ref dupTotalCount);
            }
            else
            {
                if (processed.TryAdd(stream.Id, true))
                {
                    _ = groupLookup.TryGetValue(stream.Group, out ChannelGroup? group);

                    if (!existingLookup.TryGetValue(stream.Id, out SMStream? existingStream))
                    {
                        ProcessNewStream(stream, group?.IsHidden ?? false, m3uFile.Name, index);
                        stream.ShortSMStreamId = UniqueHexGenerator.GenerateUniqueHex(generatedIdsDict);
                        toWrite.Add(stream);
                    }
                    else
                    {
                        if (ProcessExistingStream(stream, existingStream, m3uFile, index))
                        {
                            if (string.IsNullOrEmpty(existingStream.ShortSMStreamId) || existingStream.ShortSMStreamId == UniqueHexGenerator.SMChannelIdEmpty)
                            {
                                existingStream.ShortSMStreamId = UniqueHexGenerator.GenerateUniqueHex(generatedIdsDict);
                            }
                            toUpdate.Add(existingStream);
                        }
                    }
                }
            }

            _ = Interlocked.Increment(ref processedCount);
            if (processedCount % 20000 == 0)
            {
                logger.LogInformation("Processing {processedCount}/{streams.Count} streams, adding {toWrite.Count}, updating: {toUpdate.Count}", processedCount, streams.Count, toWrite.Count, toUpdate.Count);
            }
        });

        // Final batch processing
        ProcessBatches(toWrite, toUpdate);
        logger.LogInformation("Processed {processedCount} streams, inserted: {toWrite.Count}, updated: {toUpdate.Count}", processedCount, toWrite.Count, toUpdate.Count);
        return (toWrite.Count, dupTotalCount);
    }
    private static bool ProcessExistingStream(SMStream stream, SMStream existingStream, M3UFile m3uFile, int index)
    {
        bool changed = false;

        if (existingStream.M3UFileId != m3uFile.Id)
        {
            existingStream.M3UFileId = m3uFile.Id;
        }

        if (string.IsNullOrEmpty(existingStream.M3UFileName) || existingStream.M3UFileName != m3uFile.Name)
        {
            changed = true;
            existingStream.M3UFileName = m3uFile.Name;
        }

        if (existingStream.ChannelNumber != stream.ChannelNumber)
        {
            changed = true;
            existingStream.ChannelNumber = stream.ChannelNumber;
        }

        if (existingStream.Group != stream.Group)
        {
            changed = true;
            existingStream.Group = stream.Group;
        }

        if (existingStream.EPGID != stream.EPGID)
        {
            changed = true;
            existingStream.EPGID = stream.EPGID;
        }

        if (existingStream.Logo != stream.Logo)
        {
            changed = true;

            existingStream.Logo = stream.Logo;
        }

        if (existingStream.Url != stream.Url)
        {
            changed = true;

            existingStream.Url = stream.Url;
        }

        if (existingStream.Name != stream.Name)
        {
            changed = true;

            existingStream.Name = stream.Name;
        }

        if (existingStream.ShortSMStreamId != stream.ShortSMStreamId)
        {
            //changed = true;

            //existingStream.SMStreamId = stream.SMStreamId;
        }

        if (existingStream.FilePosition != index)
        {
            changed = true;

            existingStream.FilePosition = index;
        }

        return changed;
    }
    private static void ProcessNewStream(SMStream stream, bool? groupIsHidden, string mu3FileName, int index)
    {
        if (groupIsHidden is not null)
        {
            stream.IsHidden = (bool)groupIsHidden;
        }
        stream.FilePosition = index;
        stream.M3UFileName = mu3FileName;
    }
    private void ProcessBatches(ConcurrentBag<SMStream> toWrite, ConcurrentBag<SMStream> toUpdate)
    {
        if (toWrite.IsEmpty && toUpdate.IsEmpty)
        {
            return;
        }
        int totalToWrite = toWrite.Count;
        int totalToUpdate = toUpdate.Count;
        int batchWriteCount = 0;
        int batchUpdateCount = 0;

        // Convert to a list and process in chunks
        List<SMStream> writeList = [.. toWrite];
        List<SMStream> updateList = [.. toUpdate];

        const int batchSize = 500;

        if (writeList.Count > 0)
        {
            logger.LogInformation("Inserting {writeList.Count} new streams in the DB", writeList.Count);

            for (int i = 0; i < writeList.Count; i += batchSize)
            {
                List<SMStream> batch = writeList.Skip(i).Take(batchSize).ToList();
                repositoryWrapper.SMStream.BulkInsert(batch);
                //await repositoryWrapper.SMStream.BulkInsertEntitiesAsync(batch);
                batchWriteCount += batch.Count;

                if (batchWriteCount % 5000 == 0)
                {
                    logger.LogInformation("Inserted {batchWriteCount}/{totalToWrite} new streams into DB", batchWriteCount, totalToWrite);
                }
            }
        }

        if (updateList.Count > 0)
        {
            logger.LogInformation("Updating {updateList.Count} streams in DB", updateList.Count);
            for (int i = 0; i < updateList.Count; i += batchSize)
            {
                List<SMStream> batch = updateList.Skip(i).Take(batchSize).ToList();
                //await repositoryWrapper.SMStream.BulkUpdateAsync(batch);
                repositoryWrapper.SMStream.BulkUpdate(batch);
                batchUpdateCount += batch.Count;

                if (batchUpdateCount % 5000 == 0)
                {
                    logger.LogInformation("Updated {batchUpdateCount}/{totalToUpdate} streams in DB", batchUpdateCount, totalToUpdate);
                }
            }
        }
    }
}
