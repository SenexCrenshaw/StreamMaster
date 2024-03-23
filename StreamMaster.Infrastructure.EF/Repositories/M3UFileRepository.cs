using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using StreamMaster.Domain.Configuration;

using System.Collections.Concurrent;
using System.Diagnostics;

namespace StreamMaster.Infrastructure.EF.Repositories;

/// <summary>
/// Provides methods for performing CRUD operations on M3UFile entities.
/// </summary>
public class M3UFileRepository(ILogger<M3UFileRepository> intLogger, RepositoryWrapper repositoryWrapper, IJobStatusService jobStatusService, IRepositoryContext repositoryContext, IOptionsMonitor<Setting> intSettings, IMapper mapper)
    : RepositoryBase<M3UFile>(repositoryContext, intLogger, intSettings), IM3UFileRepository
{

    public PagedResponse<M3UFileDto> CreateEmptyPagedResponse()
    {
        return PagedExtensions.CreateEmptyPagedResponse<M3UFileDto>(Count());
    }

    [LogExecutionTimeAspect]
    public async Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerProcessM3U(M3UFileId);
        if (jobManager.IsRunning)
        {
            return null;
        }

        jobManager.Start();
        M3UFile? m3uFile = null;
        try
        {
            m3uFile = await GetM3UFile(M3UFileId);
            if (m3uFile == null)
            {
                logger.LogCritical("Could not find M3U file");
                jobManager.SetError();
                return null;
            }

            (List<SMStream>? streams, int streamCount) = await ProcessStreams(m3uFile).ConfigureAwait(false);
            if (streams == null)
            {
                logger.LogCritical("Error while processing M3U file, bad format");
                jobManager.SetError();
                return null;
            }

            if (!ForceRun && !ShouldUpdate(m3uFile, m3uFile.VODTags))
            {
                jobManager.SetSuccessful();
                return m3uFile;
            }

            await ProcessAndUpdateStreams(m3uFile, streams, streamCount);
            await UpdateChannelGroups(streams);

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
            if (m3uFile != null)
            {
                m3uFile.LastUpdated = SMDT.UtcNow;
                UpdateM3UFile(m3uFile);
                await SaveChangesAsync();
            }
        }
    }
    private async Task UpdateChannelGroups(List<SMStream> streams)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<string> newGroups = streams.Where(a => a.Group is not null and not "").Select(a => a.Group).Distinct().ToList();
        List<ChannelGroup> channelGroups = await repositoryWrapper.ChannelGroup.GetChannelGroups();

        await CreateNewChannelGroups(newGroups, channelGroups);

        logger.LogInformation($"Updating channel groups took {sw.Elapsed.TotalSeconds} seconds");
    }

    private async Task CreateNewChannelGroups(List<string> newGroups, List<ChannelGroup> existingGroups)
    {
        foreach (string? group in newGroups)
        {
            if (!existingGroups.Any(a => a.Name == group))
            {
                // await channelGroupService.CreateChannelGroup(group, false).ConfigureAwait(false);
            }
        }
    }

    [LogExecutionTimeAspect]
    private async Task ProcessAndUpdateStreams(M3UFile m3uFile, List<SMStream> streams, int streamCount)
    {
        await RemoveMissingStreams(streams, m3uFile.Id);

        List<SMStream> existing = await repositoryWrapper.SMStream.GetQuery().Where(a => a.M3UFileId == m3uFile.Id).ToListAsync().ConfigureAwait(false);

        List<ChannelGroup> groups = await repositoryWrapper.ChannelGroup.GetChannelGroups();

        ProcessStreamsConcurrently(streams, existing, groups, m3uFile);

        m3uFile.LastUpdated = SMDT.UtcNow;
        if (m3uFile.StationCount != streamCount)
        {
            m3uFile.StationCount = streamCount;
        }

        UpdateM3UFile(m3uFile);
        await SaveChangesAsync();
    }

    /// <summary>
    /// Removes streams that are not present in the provided list from the database, along with their related entities.
    /// </summary>
    /// <param name="streams">The list of streams to retain.</param>
    /// <param name="m3uFileId">The identifier for the M3U file associated with these streams.</param>
    /// <param name="cancellationToken">A token to cancel the current operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="Exception">Throws an exception if the operation fails.</exception>
    private async Task RemoveMissingStreams(List<SMStream> streams, int m3uFileId)
    {
        try
        {
            List<string> streamIds = streams.Select(a => a.Id).ToList();
            List<string> toDeleteStreamIds = await repositoryWrapper.SMStream
                .GetQuery(a => a.M3UFileId == m3uFileId && !streamIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            if (toDeleteStreamIds.Count != 0)
            {
                logger.LogInformation("Preparing to delete VideoStreamLink and StreamGroupVideoStream for {StreamIds}", toDeleteStreamIds);
                await DeleteRelatedVideoStreamLinks(toDeleteStreamIds);
                await DeleteSMStreams(toDeleteStreamIds);
            }
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
        await repositoryWrapper.SMChannelStreamLink.BulkDeleteAsync(toDelete).ConfigureAwait(false);
    }

    private async Task DeleteSMStreams(List<string> streamIds)
    {
        IQueryable<SMStream> toDelete = repositoryWrapper.SMStream.GetQuery(a => streamIds.Contains(a.Id));
        await repositoryWrapper.SMStream.BulkDeleteAsync(toDelete).ConfigureAwait(false);
    }

    [LogExecutionTimeAspect]
    private async Task<(List<SMStream>? streams, int streamCount)> ProcessStreams(M3UFile m3uFile)
    {
        Stopwatch sw = Stopwatch.StartNew();

        List<SMStream>? streams = await m3uFile.GetSMStreamsM3U(logger).ConfigureAwait(false);

        int streamsCount = 0;
        if (streams != null)
        {
            streamsCount = GetRealSMStreamCount(streams);
            streams = RemoveIgnoredStreams(streams);
            streams = RemoveDuplicates(streams);
        }
        sw.Stop();
        logger.LogInformation($"Processing M3U {streamsCount}, streams took {sw.Elapsed.TotalSeconds} seconds");
        return (streams, streamsCount);
    }

    private static int GetRealSMStreamCount(List<SMStream> streams)
    {
        List<string> ids = streams.Select(a => a.Id).Distinct().ToList();
        return ids.Count;
    }
    private List<SMStream> RemoveIgnoredStreams(List<SMStream> streams)
    {
        if (Settings.NameRegex.Count == 0)
        {
            return streams;
        }

        foreach (string regex in Settings.NameRegex)
        {
            List<SMStream> toIgnore = ListHelper.GetMatchingProperty(streams, "name", regex);
            logger.LogInformation($"Ignoring {toIgnore.Count} streams with regex {regex}");
            _ = streams.RemoveAll(toIgnore.Contains);
        }

        return streams;
    }

    private List<SMStream> RemoveDuplicates(List<SMStream> streams)
    {
        List<SMStream> cleanStreams = streams.GroupBy(s => s.Id)
                  .Select(g => g.First())
                  .ToList();

        logger.LogInformation($"Removed {streams.Count - cleanStreams.Count} duplicate streams");

        return cleanStreams;
    }

    private List<VideoStream> RemoveIgnoredStreams(List<VideoStream> streams)
    {
        if (Settings.NameRegex.Any())
        {
            foreach (string regex in Settings.NameRegex)
            {
                List<VideoStream> toIgnore = ListHelper.GetMatchingProperty(streams, "Tvg_name", regex);
                logger.LogInformation($"Ignoring {toIgnore.Count} streams with regex {regex}");
                _ = streams.RemoveAll(toIgnore.Contains);
            }
        }

        return streams;
    }


    private static int GetRealStreamCount(List<VideoStream> streams)
    {
        List<string> ids = streams.Select(a => a.Id).Distinct().ToList();
        return ids.Count;
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

    /// <inheritdoc/>
    public void CreateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to create a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }
        Create(m3uFile);
        logger.LogInformation($"Created M3UFile with ID: {m3uFile.Id}.");
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
        logger.LogInformation($"M3UFile with Name {m3uFile.Name} was deleted.");
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
    public async Task<PagedResponse<M3UFileDto>> GetPagedM3UFiles(M3UFileParameters parameters)
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

        logger.LogInformation($"Updated M3UFile with ID: {m3uFile.Id}.");
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


    public async Task<M3UFileDto?> ChangeM3UFileName(int M3UFileId, string newName)
    {
        M3UFile? m3UFile = await FirstOrDefaultAsync(a => a.Id == M3UFileId).ConfigureAwait(false);
        if (m3UFile == null)
        {
            return null;
        }
        m3UFile.Name = newName;
        Update(m3UFile);
        await RepositoryContext.SaveChangesAsync().ConfigureAwait(false);
        return mapper.Map<M3UFileDto>(m3UFile);
    }

    public IQueryable<M3UFile> GetM3UFileQuery()
    {
        return GetQuery();
    }

    private void ProcessStreamsConcurrently(List<SMStream> streams, List<SMStream> existing, List<ChannelGroup> groups, M3UFile m3uFile)
    {
        int totalCount = streams.Count;
        Dictionary<string, SMStream> existingLookup = existing.ToDictionary(a => a.Id, a => a);
        Dictionary<string, ChannelGroup> groupLookup = groups.ToDictionary(g => g.Name, g => g);
        ConcurrentDictionary<string, bool> processed = new();

        ConcurrentBag<SMStream> toWrite = [];
        ConcurrentBag<SMStream> toUpdate = [];

        int processedCount = 0;

        ConcurrentDictionary<string, byte> generatedIdsDict = new();

        foreach (SMStream stream in streams)
        {
            generatedIdsDict.TryAdd(stream.ShortId, 0);
        }

        _ = Parallel.ForEach(streams.Select((stream, index) => (stream, index)), tuple =>
        {
            SMStream stream = tuple.stream;
            int index = tuple.index;

            if (processed.TryAdd(stream.Id, true))
            {
                _ = groupLookup.TryGetValue(stream.Group, out ChannelGroup? group);

                if (!existingLookup.TryGetValue(stream.Id, out SMStream? existingStream))
                {
                    ProcessNewStream(stream, group?.IsHidden ?? false, m3uFile.Name, index);
                    stream.ShortId = UniqueHexGenerator.GenerateUniqueHex(generatedIdsDict);
                    toWrite.Add(stream);
                }
                else
                {
                    if (ProcessExistingStream(stream, existingStream, m3uFile, index))
                    {
                        if (string.IsNullOrEmpty(existingStream.ShortId) || existingStream.ShortId == UniqueHexGenerator.ShortIdEmpty)
                        {
                            existingStream.ShortId = UniqueHexGenerator.GenerateUniqueHex(generatedIdsDict);
                        }
                        toUpdate.Add(existingStream);
                    }
                }

                _ = Interlocked.Increment(ref processedCount);
                if (processedCount % 20000 == 0)
                {
                    logger.LogInformation($"Processed {processedCount}/{totalCount} streams, adding {toWrite.Count}, updating: {toUpdate.Count}");
                }
            }
        });


        // Final batch processing
        ProcessBatches(toWrite, toUpdate);
        logger.LogInformation($"Finished Found {processedCount} streams, inserted: {toWrite.Count}, updated: {toUpdate.Count}");
    }
    private bool ProcessExistingStream(SMStream stream, SMStream existingStream, M3UFile m3uFile, int index)
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

        if (existingStream.Name != stream.Name)
        {
            changed = true;

            existingStream.Name = stream.Name;
        }

        if (existingStream.ShortId != stream.ShortId)
        {
            changed = true;

            existingStream.ShortId = stream.ShortId;
        }

        if (existingStream.FilePosition != index)
        {
            changed = true;

            existingStream.FilePosition = index;
        }

        return changed;
    }
    private void ProcessNewStream(SMStream stream, bool? groupIsHidden, string mu3FileName, int index)
    {
        if (groupIsHidden is not null)
        {
            stream.IsHidden = (bool)groupIsHidden;
        }
        stream.FilePosition = index;
        stream.M3UFileName = mu3FileName;
    }

    /// <summary>
    /// Processes batches of streams for insertion and update in the database. Streams are handled
    /// in chunks to optimize database interactions. The method logs progress at intervals
    /// for both insertions and updates, providing visibility into the operation's progress.
    /// </summary>
    /// <param name="toWrite">A ConcurrentBag of <see cref="SMStream"/> instances to be inserted into the database.</param>
    /// <param name="toUpdate">A ConcurrentBag of <see cref="SMStream"/> instances to be updated in the database.</param>
    /// <remarks>
    /// The method divides streams into batches to manage memory usage and improve database
    /// transaction efficiency. It performs bulk insertions and updates by utilizing the
    /// repository wrapper's bulk operation methods. Logging is performed throughout the process
    /// to monitor progress. It is assumed that the repository wrapper is thread-safe or that
    /// the method is called in a thread-safe context.
    /// </remarks>
    /// <summary>
    /// Processes batches of streams for insertion and update in the database. Streams are handled
    /// in chunks to optimize database interactions. The method logs progress at intervals
    /// for both insertions and updates, providing visibility into the operation's progress.
    /// </summary>
    /// <param name="toWrite">A ConcurrentBag of <see cref="SMStream"/> instances to be inserted into the database.</param>
    /// <param name="toUpdate">A ConcurrentBag of <see cref="SMStream"/> instances to be updated in the database.</param>
    /// <remarks>
    /// The method divides streams into batches to manage memory usage and improve database
    /// transaction efficiency. It performs bulk insertions and updates by utilizing the
    /// repository wrapper's bulk operation methods. Logging is performed throughout the process
    /// to monitor progress. It is assumed that the repository wrapper is thread-safe or that
    /// the method is called in a thread-safe context.
    /// </remarks>
    private void ProcessBatches(ConcurrentBag<SMStream> toWrite, ConcurrentBag<SMStream> toUpdate)
    {
        int totalToWrite = toWrite.Count;
        int totalToUpdate = toUpdate.Count;
        int batchWriteCount = 0;
        int batchUpdateCount = 0;

        // Convert to a list and process in chunks
        List<SMStream> writeList = [.. toWrite];
        List<SMStream> updateList = [.. toUpdate];

        int batchSize = 500;

        if (writeList.Count != 0)
        {
            logger.LogInformation($"Inserting {writeList.Count} new streams in the DB");

            for (int i = 0; i < writeList.Count; i += batchSize)
            {

                List<SMStream> batch = writeList.Skip(i).Take(batchSize).ToList();
                repositoryWrapper.SMStream.BulkInsert(batch);
                batchWriteCount += batch.Count;

                if (batchWriteCount % 5000 == 0)
                {
                    logger.LogInformation($"Inserted {batchWriteCount}/{totalToWrite} new streams into DB");
                }
            }
        }

        if (updateList.Any())
        {
            logger.LogInformation($"Updating {updateList.Count} streams in DB");
            for (int i = 0; i < updateList.Count; i += batchSize)
            {
                List<SMStream> batch = updateList.Skip(i).Take(batchSize).ToList();
                repositoryWrapper.SMStream.BulkUpdate(batch);
                batchUpdateCount += batch.Count;

                if (batchUpdateCount % 5000 == 0)
                {
                    logger.LogInformation($"Updated {batchUpdateCount}/{totalToUpdate} streams in DB");
                }
            }
        }
    }

}
