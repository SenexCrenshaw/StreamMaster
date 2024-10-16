using StreamMaster.Application.M3UFiles.Commands;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace StreamMaster.Application.M3UFiles;

public class M3UFileService(ILogger<M3UFileService> logger, IFileUtilService fileUtilService, IM3UToSMStreamsService m3UtoSMStreamsService, IJobStatusService jobStatusService, IOptionsMonitor<Setting> _settings, IMessageService messageService, IRepositoryWrapper repositoryWrapper, IRepositoryContext repositoryContext)
    : IM3UFileService
{
    public async Task<DataResponse<List<M3UFileDto>>> GetM3UFilesNeedUpdatingAsync()
    {
        List<M3UFileDto> M3UFilesToUpdated = await repositoryWrapper.M3UFile.GetM3UFilesNeedUpdatingAsync();
        return DataResponse<List<M3UFileDto>>.Success(M3UFilesToUpdated);
    }

    public async Task<M3UFile?> ProcessM3UFile(int M3UFileId, bool ForceRun = false)
    {
        JobStatusManager jobManager = jobStatusService.GetJobManagerProcessM3U(M3UFileId);
        if (jobManager.IsRunning)
        {
            return null;
        }

        jobManager.Start();
        M3UFile? m3uFile;
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
            m3uFile = await GetM3UFileAsync(M3UFileId);
            if (m3uFile == null)
            {
                logger.LogCritical("Could parse M3U file M3UFileId: {M3UFileId}", M3UFileId);
                jobManager.SetError();
                return null;
            }

            if (!ForceRun && !ShouldUpdate(m3uFile, m3uFile.VODTags))
            {
                jobManager.SetSuccessful();
                return m3uFile;
            }
            await ProcessAndUpdateStreams(m3uFile);

            m3uFile.LastUpdated = SMDT.UtcNow;
            await UpdateM3UFile(m3uFile);

            jobManager.SetSuccessful();
            stopwatch.Stop();
            logger.LogInformation("ProcessM3UFile {name} took {elapsed}ms", m3uFile.Name, stopwatch.ElapsedMilliseconds);

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

    public async Task UpdateM3UFile(M3UFile m3uFile)
    {
        if (m3uFile == null)
        {
            logger.LogError("Attempted to update a null M3UFile.");
            throw new ArgumentNullException(nameof(m3uFile));
        }
        repositoryWrapper.M3UFile.Update(m3uFile);
        m3uFile.WriteJSON();
        await repositoryWrapper.SaveAsync();
        logger.LogInformation("Updated M3UFile with ID: {m3uFile.Id}.", m3uFile.Id);
    }

    private async Task UpdateChannelGroups(List<string> newGroups)
    {
        Stopwatch sw = Stopwatch.StartNew();

        //List<string> newGroups = streams.Where(a => a.Group is not null and not "").Select(a => a.Group).Distinct().ToList();
        List<ChannelGroup> channelGroups = await repositoryWrapper.ChannelGroup.GetQuery().ToListAsync();

        _ = CreateNewChannelGroups(newGroups, channelGroups);

        logger.LogInformation("Updating channel groups took {sw.Elapsed.TotalSeconds} seconds", sw.Elapsed.TotalSeconds);
    }

    private APIResponse CreateNewChannelGroups(List<string> newGroups, List<ChannelGroup> existingGroups)
    {
        IEnumerable<string> existingNames = existingGroups.Select(a => a.Name);
        List<string> toCreate = newGroups.Where(a => !existingNames.Contains(a)).ToList();

        return repositoryWrapper.ChannelGroup.CreateChannelGroups(toCreate, true);
    }

    public (string fullName, string fullNameWithExtension) GetFileName(string name)
    {
        FileDefinition fd = FileDefinitions.M3U;

        string fullNameWithExtension = name + fd.DefaultExtension;
        string compressedFileName = fileUtilService.CheckNeedsCompression(fullNameWithExtension);
        string fullName = Path.Combine(fd.DirectoryLocation, compressedFileName);
        return (fullName, fullNameWithExtension);
    }

    public (M3UFile m3uFile, string fullName) CreateM3UFileBase(
    string name,
    int maxStreamCount,
    string? urlSource,
    List<string>? vodTags,
    int? hoursToUpdate,
    bool? syncChannels,
    string? defaultStreamGroupName,
    bool? autoSetChannelNumbers,
    int? startingChannelNumber,
    M3UKey? m3uKey,
    M3UField? m3uField)
    {
        (string fullName, string fullNameWithExtension) = GetFileName(name);

        M3UFile m3uFile = new()
        {
            Name = name,
            Url = urlSource,
            MaxStreamCount = maxStreamCount,
            Source = fullNameWithExtension,
            VODTags = vodTags ?? [],
            HoursToUpdate = hoursToUpdate ?? 72,
            SyncChannels = syncChannels ?? false,
            DefaultStreamGroupName = defaultStreamGroupName,
            AutoSetChannelNumbers = autoSetChannelNumbers ?? false,
            StartingChannelNumber = startingChannelNumber ?? 1,
            M3UKey = m3uKey ?? M3UKey.URL,
            M3UName = m3uField ?? M3UField.Name,
        };

        return (m3uFile, fullName);
    }

    public (M3UFile m3uFile, string fullName) CreateM3UFile(CreateM3UFileRequest request)
    {
        return CreateM3UFileBase(
            request.Name,
            request.MaxStreamCount,
            request.UrlSource,
            request.VODTags,
            request.HoursToUpdate,
            request.SyncChannels,
            request.DefaultStreamGroupName,
            request.AutoSetChannelNumbers,
            request.StartingChannelNumber,
            request.M3UKey,
            request.M3UName
        );
    }

    public (M3UFile m3uFile, string fullName) CreateM3UFile(CreateM3UFileFromFormRequest request)
    {
        return CreateM3UFileBase(
            request.Name,
            request.MaxStreamCount ?? 0,
            null, // URL source is not provided in the form request
            request.VODTags,
            request.HoursToUpdate,
            request.SyncChannels,
            request.DefaultStreamGroupName,
            request.AutoSetChannelNumbers,
            request.StartingChannelNumber,
            request.M3UKey,
            request.M3UName
        );
    }

    [LogExecutionTimeAspect]
    private async Task ProcessAndUpdateStreams(M3UFile m3uFile)
    {
        IAsyncEnumerable<SMStream?> streams = m3UtoSMStreamsService.GetSMStreamsFromM3U(m3uFile);
        if (streams == null)
        {
            logger.LogError("Could not get streams from M3U file {m3uFile.Name}", m3uFile.Name);
            return;
        }

        (List<string> cgs, int count, List<DupInfo>? dupInfos, int removedCount) = await ProcessStreamsConcurrently(streams!, m3uFile);

        m3uFile.LastUpdated = SMDT.UtcNow;
        m3uFile.StreamCount = count;

        if (dupInfos.Count > 0)
        {
            string jsonText = JsonSerializer.Serialize(dupInfos, BuildInfo.JsonIndentOptions);
            FileUtil.WriteJSON(m3uFile.Name + "_duplicates.json", jsonText, BuildInfo.DupDataFolder);
        }

        logger.LogInformation("Processed {m3uFile.Name} : {streams.Count} total streams in file, removed {streamIds.Count} missing steams and ignored {dupStreamCount} duplicate streams", m3uFile.Name, count, removedCount, dupInfos.Count);
        await messageService.SendSuccess($"{count} total streams in file \r\nremoved {removedCount} missing steams and ignored {dupInfos.Count} duplicate streams", $"Processed M3U {m3uFile.Name}");

        await UpdateM3UFile(m3uFile);
        _ = await repositoryWrapper.SaveAsync();

        await UpdateChannelGroups(cgs);
    }

    private async Task<(List<string> cgs, int count, List<DupInfo> dupInfos, int removedCount)> ProcessStreamsConcurrently(IAsyncEnumerable<SMStream> streams, M3UFile m3uFile)
    {
        List<DupInfo> dupInfos = [];

        int processedCount = 0;

        Dictionary<string, bool> groupLookup = await repositoryWrapper.ChannelGroup.GetQuery().ToDictionaryAsync(g => g.Name, g => g.IsHidden);
        //HashSet<string> existingStreamIds = (await repositoryWrapper.SMStream.GetQuery(a => a.M3UFileId == m3uFile.Id).Select(a => a.Id).ToListAsync().ConfigureAwait(false)).ToHashSet();

        ConcurrentDictionary<string, bool> processed = new();

        HashSet<string> Cgs = [];

        bool createChannel = m3uFile.SyncChannels;

        int streamGroupId = 0;

        if (createChannel)
        {
            StreamGroup? sg = await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Name == m3uFile.DefaultStreamGroupName);
            if (sg != null)
            {
                streamGroupId = sg.Id;
            }
        }

        int batchSize = _settings.CurrentValue.DBBatchSize;// BuildInfo.DBBatchSize;
        List<SMStream> batch = [];

        repositoryContext.ExecuteSqlRaw($"UPDATE public.\"SMStreams\" SET \"NeedsDelete\" = true WHERE \"M3UFileId\" = {m3uFile.Id}");

        Stopwatch stopwatch = Stopwatch.StartNew();
        Stopwatch mainStopwatch = Stopwatch.StartNew();
        await foreach (SMStream stream in streams)
        {
            Interlocked.Increment(ref processedCount);
            if (_settings.CurrentValue.NameRegex.Count > 0)
            {
                foreach (string regex in _settings.CurrentValue.NameRegex)
                {
                    if (Regex.IsMatch(stream.Name, regex, RegexOptions.IgnoreCase))
                    {
                        continue;
                    }
                }
            }

            if (processed.TryAdd(stream.Id, true))
            {
                Cgs.Add(stream.Group);
                groupLookup.TryGetValue(stream.Group, out bool hidden);

                stream.IsHidden = hidden;
                stream.FilePosition = processedCount;
                stream.M3UFileName = m3uFile.Name;

                if (m3uFile.AutoSetChannelNumbers)
                {
                    stream.ChannelNumber = processedCount + m3uFile.StartingChannelNumber;
                }

                batch.Add(stream);

                if (batch.Count >= batchSize)
                {
                    List<int> smChannelIds = await ExecuteCreateSmStreamsAndChannelsAsync(batch, m3uFile.Id, m3uFile.Name, streamGroupId, m3uFile.SyncChannels);
                    if (smChannelIds.Count > 0)
                    {
                        await repositoryWrapper.SMChannel.AutoSetEPGFromIds(smChannelIds, CancellationToken.None);
                        await repositoryWrapper.SaveAsync();
                    }
                    batch.Clear();
                }
            }
            else
            {
                dupInfos.Add(new DupInfo
                {
                    Id = stream.Id,
                    Name = stream.Name,
                    M3UFileName = m3uFile.Name,
                    FilePosition = stream.FilePosition
                });
            }

            if (processedCount % 10000 == 0)
            {
                int streamPerSecond = (int)(10000 / stopwatch.Elapsed.TotalSeconds);
                logger.LogInformation("Processing {processedCount} streams in {elapsed}ms / {streamPerSecond} streams per second ", processedCount, stopwatch.ElapsedMilliseconds, streamPerSecond);
                stopwatch.Restart();
            }
        }

        // Process any remaining streams in the batch
        if (batch.Count > 0)
        {
            List<int> smChannelIds = await ExecuteCreateSmStreamsAndChannelsAsync(batch, m3uFile.Id, m3uFile.Name, streamGroupId, m3uFile.SyncChannels);
            if (smChannelIds.Count > 0)
            {
                await repositoryWrapper.SMChannel.AutoSetEPGFromIds(smChannelIds, CancellationToken.None);
                await repositoryWrapper.SaveAsync();
            }
            batch.Clear();
        }
        batch.Clear();

        IQueryable<SMStream> toDelete = repositoryWrapper.SMStream.GetQuery(a => a.M3UFileId == m3uFile.Id && a.NeedsDelete);
        if (await toDelete.AnyAsync().ConfigureAwait(false))
        {
            await repositoryWrapper.SMStream.BulkDeleteAsync(toDelete).ConfigureAwait(false);
        }
        mainStopwatch.Stop();
        logger.LogInformation("Processed {processedCount} streams in {elapsed}ms", processedCount, mainStopwatch.ElapsedMilliseconds);
        return (Cgs.ToList(), processedCount, dupInfos, toDelete.Count());
    }

    private async Task<List<int>> ExecuteCreateSmStreamsAndChannelsAsync(List<SMStream> streams, int m3uFileId, string m3uFileName, int streamGroupId, bool createChannels)
    {
        // Generate the SQL command manually
        string sqlCommand = GenerateBatchSqlCommandForDebugging(streams, m3uFileId, m3uFileName, streamGroupId, createChannels);

        try
        {
            // Execute the SQL command and retrieve the list of channel IDs
            return await repositoryContext.SqlQueryRaw<int>(sqlCommand).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute SQL command: {sqlCommand}", sqlCommand);
            // throw;
        }

        return [];
    }

    private static string GenerateBatchSqlCommandForDebugging(List<SMStream> streams, int m3uFileId, string m3uFileName, int streamGroupId, bool createChannels)
    {
        string[] ids = streams.Select(s => $"'{EscapeString(s.Id)}'").ToArray();
        string[] filePositions = streams.Select(s => s.FilePosition.ToString()).ToArray();
        string[] channelNumbers = streams.Select(s => s.ChannelNumber.ToString()).ToArray();
        string[] groups = streams.Select(s => $"'{EscapeString(s.Group)}'").ToArray();
        string[] epgIds = streams.Select(s => $"'{EscapeString(s.EPGID)}'").ToArray();
        string[] logos = streams.Select(s => $"'{EscapeString(s.Logo)}'").ToArray();
        string[] names = streams.Select(s => $"'{EscapeString(s.Name)}'").ToArray();
        string[] urls = streams.Select(s => $"'{EscapeString(s.Url)}'").ToArray();
        string[] stationIds = streams.Select(s => $"'{EscapeString(s.StationId)}'").ToArray();
        string[] channelIds = streams.Select(s => $"'{EscapeString(s.ChannelId)}'").ToArray();
        string[] channelNames = streams.Select(s => $"'{EscapeString(s.ChannelName)}'").ToArray();
        string[] isHidden = streams.Select(s => s.IsHidden.ToString().ToUpper()).ToArray(); // Add IsHidden as a boolean array
        string[] tvgNames = streams.Select(s => $"'{EscapeString(s.TVGName)}'").ToArray(); // Add TVGName array

        // Construct the SQL command to call the function
        string sqlCommand = $@"
    SELECT * FROM public.create_or_update_smstreams_and_channels(
        ARRAY[{string.Join(", ", ids)}]::TEXT[],
        ARRAY[{string.Join(", ", filePositions)}]::INTEGER[],
        ARRAY[{string.Join(", ", channelNumbers)}]::INTEGER[],
        ARRAY[{string.Join(", ", groups)}]::CITEXT[],
        ARRAY[{string.Join(", ", epgIds)}]::CITEXT[],
        ARRAY[{string.Join(", ", logos)}]::CITEXT[],
        ARRAY[{string.Join(", ", names)}]::CITEXT[],
        ARRAY[{string.Join(", ", urls)}]::CITEXT[],
        ARRAY[{string.Join(", ", stationIds)}]::CITEXT[],
        ARRAY[{string.Join(", ", channelIds)}]::CITEXT[],
        ARRAY[{string.Join(", ", channelNames)}]::CITEXT[],
        ARRAY[{string.Join(", ", isHidden)}]::BOOLEAN[], -- Include the IsHidden array
        ARRAY[{string.Join(", ", tvgNames)}]::CITEXT[], -- Include the TVGName array
        {m3uFileId}, -- p_m3u_file_id as INTEGER
        '{EscapeString(m3uFileName)}'::CITEXT,
        {streamGroupId},
        {createChannels.ToString().ToUpper()}
    );
    ";

        return sqlCommand;
    }

    private static string EscapeString(string input)
    {
        return input.Replace("'", "''")
                          .Replace("{", "[")     // Escape opening curly brace
                          .Replace("}", "]");
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

    public async Task<M3UFile?> GetM3UFileAsync(int Id)
    {
        return await repositoryWrapper.M3UFile.FirstOrDefaultAsync(c => c.Id == Id, false).ConfigureAwait(false);
    }

    public async Task<List<M3UFile>> GetM3UFilesAsync()
    {
        return await repositoryWrapper.M3UFile.GetQuery().ToListAsync();
    }
}