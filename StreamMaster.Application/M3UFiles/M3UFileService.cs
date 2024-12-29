using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;

using Npgsql;

using NpgsqlTypes;

namespace StreamMaster.Application.M3UFiles;

public class M3UFileService(ILogger<M3UFileService> logger, IFileUtilService fileUtilService, IM3UToSMStreamsService m3UtoSMStreamsService, IJobStatusService jobStatusService, IOptionsMonitor<Setting> settings, IMessageService messageService, IRepositoryWrapper repositoryWrapper, IRepositoryContext repositoryContext)
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
        List<string> toCreate = [.. newGroups.Where(a => !existingNames.Contains(a))];

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
    M3UField? m3uField,
    string? M3U8OutPutProfile)
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
            M3U8OutPutProfile = M3U8OutPutProfile
        };

        return (m3uFile, fullName);
    }

    public (M3UFile m3uFile, string fullName) CreateM3UFile(string Name, int? MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, string? DefaultStreamGroupName, string? UrlSource, bool? SyncChannels, int? HoursToUpdate, int? StartingChannelNumber, bool? AutoSetChannelNumbers, List<string>? VODTags)
    {
        return CreateM3UFileBase(
            Name,
            MaxStreamCount ?? 0,
            UrlSource,
            VODTags,
            HoursToUpdate,
            SyncChannels,
            DefaultStreamGroupName,
            AutoSetChannelNumbers,
            StartingChannelNumber,
            M3UKey,
            M3UName,
            M3U8OutPutProfile
        );
    }

    public (M3UFile m3uFile, string fullName) CreateM3UFile(string Name, int? MaxStreamCount, string? M3U8OutPutProfile, M3UKey? M3UKey, M3UField? M3UName, int? StartingChannelNumber, bool? AutoSetChannelNumbers, string? DefaultStreamGroupName, int? HoursToUpdate, bool? SyncChannels, IFormFile? FormFile, List<string>? VODTags)
    {
        return CreateM3UFileBase(
            Name,
            MaxStreamCount ?? 0,
            null,
            VODTags,
            HoursToUpdate,
            SyncChannels,
            DefaultStreamGroupName,
            AutoSetChannelNumbers,
            StartingChannelNumber,
            M3UKey,
            M3UName,
            M3U8OutPutProfile
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

        (List<string> cgs, int count, List<DupInfo>? dupInfos, int removedCount) = await ProcessStreamsConcurrentlyOptimized(streams!, m3uFile);

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

    private async Task<(List<string> cgs, int count, List<DupInfo> dupInfos, int removedCount)> ProcessStreamsConcurrentlyOptimized(IAsyncEnumerable<SMStream> streams, M3UFile m3uFile)
    {
        List<DupInfo> dupInfos = [];
        HashSet<string> processed = [];
        HashSet<string> cgs = [];
        int processedCount = 0;

        // Fetch group lookup
        Dictionary<string, bool> groupLookup = await repositoryWrapper.ChannelGroup.GetQuery()
            .ToDictionaryAsync(g => g.Name, g => g.IsHidden)
            .ConfigureAwait(false);

        bool createChannel = m3uFile.SyncChannels;
        int streamGroupId = 0;

        if (createChannel)
        {
            StreamGroup? streamGroup = await repositoryWrapper.StreamGroup.FirstOrDefaultAsync(a => a.Name == m3uFile.DefaultStreamGroupName)
                .ConfigureAwait(false);
            streamGroupId = streamGroup?.Id ?? 0;
        }

        int batchSize = settings.CurrentValue.DBBatchSize;
        List<SMStream> batch = [];

        // Mark streams for deletion
        await repositoryContext.ExecuteSqlRawAsync($"UPDATE public.\"SMStreams\" SET \"NeedsDelete\" = true WHERE \"M3UFileId\" = {m3uFile.Id}")
            .ConfigureAwait(false);

        Regex? combinedRegex = CreateCombinedRegex(settings.CurrentValue.NameRegex);

        Stopwatch mainStopwatch = Stopwatch.StartNew();
        Stopwatch logStopwatch = Stopwatch.StartNew();
        //Stopwatch batchProcessStopwatch = Stopwatch.StartNew();
        //Stopwatch batchBuildStopwatch = Stopwatch.StartNew();
        int lastProcessedCount = 0;
        await foreach (SMStream? stream in streams.ConfigureAwait(false))
        {
            Interlocked.Increment(ref processedCount);

            // Periodic logging
            if (processedCount % 10_000 == 0)
            {
                int intervalCount = processedCount - lastProcessedCount;
                int intervalRate = (int)(intervalCount / logStopwatch.Elapsed.TotalSeconds);
                lastProcessedCount = processedCount;

                logger.LogInformation("Processed {intervalCount}/{processedCount} streams in {elapsed}ms at {intervalRate} streams/sec",
                    intervalCount, processedCount, logStopwatch.ElapsedMilliseconds, intervalRate);
                logStopwatch.Restart();
            }

            // Skip stream names matching the regex
            if (combinedRegex?.IsMatch(stream.Name) == true)
            {
                continue;
            }

            if (processed.Add(stream.Id)) // Use HashSet instead of ConcurrentDictionary
            {
                cgs.Add(stream.Group);

                if (groupLookup.TryGetValue(stream.Group, out bool hidden))
                {
                    stream.IsHidden = hidden;
                }

                stream.FilePosition = processedCount;
                stream.M3UFileName = m3uFile.Name;

                if (m3uFile.AutoSetChannelNumbers)
                {
                    stream.ChannelNumber = processedCount + m3uFile.StartingChannelNumber;
                }

                batch.Add(stream);

                // Process batch if full
                if (batch.Count >= batchSize)
                {
                    await ProcessBatchAsync(batch, m3uFile.Id, m3uFile.Name, streamGroupId, createChannel).ConfigureAwait(false);
                    //batchProcessStopwatch.Stop();
                    //logger.LogInformation("Processed {batch.Count} streams in {batchStopwatch.ElapsedMilliseconds} MS", batch.Count, batchProcessStopwatch.ElapsedMilliseconds);
                    //batchProcessStopwatch.Restart();
                    batch = [];
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
        }

        // Process remaining batch
        if (batch.Count > 0)
        {
            await ProcessBatchAsync(batch, m3uFile.Id, m3uFile.Name, streamGroupId, createChannel).ConfigureAwait(false);
        }

        // Remove marked streams
        IQueryable<SMStream> toDelete = repositoryWrapper.SMStream.GetQuery(a => a.M3UFileId == m3uFile.Id && a.NeedsDelete);
        int removedCount = await toDelete.CountAsync().ConfigureAwait(false);

        if (removedCount > 0)
        {
            await repositoryWrapper.SMStream.BulkDeleteAsync(toDelete).ConfigureAwait(false);
        }

        mainStopwatch.Stop();
        logger.LogInformation("Processed {processedCount} streams in {elapsed}ms", processedCount, mainStopwatch.ElapsedMilliseconds);

        return (cgs.ToList(), processedCount, dupInfos, removedCount);
    }

    private static Regex? CreateCombinedRegex(IEnumerable<string> patterns)
    {
        if (patterns?.Any() != true)
        {
            // Return a regex that never matches anything
            return null;
        }

        // Combine patterns into a single regex
        return new Regex(string.Join("|", patterns.Select(Regex.Escape)), RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    private async Task ProcessBatchAsync(List<SMStream> batch, int m3uFileId, string m3uFileName, int streamGroupId, bool createChannel)
    {
        List<int> smChannelIds = await ExecuteCreateSmStreamsAndChannelsAsync(batch, m3uFileId, m3uFileName, streamGroupId, createChannel).ConfigureAwait(false);

        if (settings.CurrentValue.AutoSetEPG && smChannelIds.Count > 0)
        {
            await repositoryWrapper.SMChannel.AutoSetEPGFromIds(smChannelIds, CancellationToken.None).ConfigureAwait(false);
            await repositoryWrapper.SaveAsync().ConfigureAwait(false);
        }
    }

    private async Task<List<int>> ExecuteCreateSmStreamsAndChannelsAsync(List<SMStream> streams, int m3uFileId, string m3uFileName, int streamGroupId, bool createChannels)
    {
        // Generate the SQL command manually
        List<NpgsqlParameter> parameters = GenerateBatchSqlCommandWithParameters(streams, m3uFileId, m3uFileName, streamGroupId, createChannels, settings.CurrentValue.AutoSetEPG);
        // string sqlCommand2 = GenerateBatchSqlCommandForDebugging(streams, m3uFileId, m3uFileName, streamGroupId, createChannels, settings.CurrentValue.AutoSetEPG);
        try
        {
            // Execute the SQL command and retrieve the list of channel IDs
            return await repositoryContext.SqlQueryRaw<int>(sqlCommand, [.. parameters]).ToListAsync();
            //return await repositoryContext.SqlQueryRaw<int>(sqlCommand2).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to execute SQL command: {sqlCommand}", sqlCommand);
            // throw;
        }

        return [];
    }

    private static string GenerateBatchSqlCommandForDebugging(List<SMStream> streams, int m3uFileId, string m3uFileName, int streamGroupId, bool createChannels, bool returnResults)
    {
        string[] ids = [.. streams.Select(s => $"'{EscapeString(s.Id)}'")];
        string[] filePositions = [.. streams.Select(s => s.FilePosition.ToString())];
        string[] channelNumbers = [.. streams.Select(s => s.ChannelNumber.ToString())];
        string[] groups = [.. streams.Select(s => $"'{EscapeString(s.Group)}'")];
        string[] epgIds = [.. streams.Select(s => $"'{EscapeString(s.EPGID)}'")];
        string[] logos = [.. streams.Select(s => $"'{EscapeString(s.Logo)}'")];
        string[] names = [.. streams.Select(s => $"'{EscapeString(s.Name)}'")];
        string[] urls = [.. streams.Select(s => $"'{EscapeString(s.Url)}'")];
        string[] stationIds = [.. streams.Select(s => $"'{EscapeString(s.StationId)}'")];
        string[] channelIds = [.. streams.Select(s => $"'{EscapeString(s.ChannelId)}'")];
        string[] channelNames = [.. streams.Select(s => $"'{EscapeString(s.ChannelName)}'")];
        string[] extIfs = [.. streams.Select(s => $"'{EscapeString(s.ExtInf ?? "-1")}'")];
        string[] isHidden = [.. streams.Select(s => s.IsHidden.ToString().ToUpper())]; // Add IsHidden as a boolean array
        string[] tvgNames = [.. streams.Select(s => $"'{EscapeString(s.TVGName)}'")]; // Add TVGName array

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
    ARRAY[{string.Join(", ", extIfs)}]::TEXT[],
    ARRAY[{string.Join(", ", isHidden)}]::BOOLEAN[],
    ARRAY[{string.Join(", ", tvgNames)}]::CITEXT[],
    {m3uFileId},
    '{EscapeString(m3uFileName)}'::CITEXT,
    {streamGroupId},
    {createChannels.ToString().ToUpper()},
    {returnResults.ToString().ToUpper()}
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

    private const string sqlCommand = @"
SELECT * FROM public.create_or_update_smstreams_and_channels(
    @p_ids::TEXT[],
    @p_file_positions::INTEGER[],
    @p_channel_numbers::INTEGER[],
    @p_groups::CITEXT[],
    @p_epgids::CITEXT[],
    @p_logos::CITEXT[],
    @p_names::CITEXT[],
    @p_urls::CITEXT[],
    @p_station_ids::CITEXT[],
    @p_channel_ids::CITEXT[],
    @p_channel_names::CITEXT[],
    @p_extifs::TEXT[],
    @p_is_hidden::BOOLEAN[],
    @p_tvg_names::CITEXT[],
    @p_m3u_file_id,
    @p_m3u_file_name::CITEXT,
    @p_stream_group_id,
    @p_create_channels,
    @p_return_results
);";

    private static List<NpgsqlParameter> GenerateBatchSqlCommandWithParameters(
    List<SMStream> streams, int m3uFileId, string m3uFileName, int streamGroupId, bool createChannels, bool returnResults)
    {
        // Define the SQL command with placeholders

#pragma warning disable RCS1130 // Bitwise operation on enum without Flags attribute
        List<NpgsqlParameter> parameters =
        [
        new NpgsqlParameter("p_ids", NpgsqlDbType.Array | NpgsqlDbType.Text) { Value = streams.Select(s => s.Id).ToArray() },
        new NpgsqlParameter("p_file_positions", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = streams.Select(s => s.FilePosition).ToArray() },
        new NpgsqlParameter("p_channel_numbers", NpgsqlDbType.Array | NpgsqlDbType.Integer) { Value = streams.Select(s => s.ChannelNumber).ToArray() },
        new NpgsqlParameter("p_groups", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.Group).ToArray() },
        new NpgsqlParameter("p_epgids", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.EPGID).ToArray() },
        new NpgsqlParameter("p_logos", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.Logo).ToArray() },
        new NpgsqlParameter("p_names", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.Name).ToArray() },
        new NpgsqlParameter("p_urls", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.Url).ToArray() },
        new NpgsqlParameter("p_station_ids", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.StationId).ToArray() },
        new NpgsqlParameter("p_channel_ids", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.ChannelId).ToArray() },
        new NpgsqlParameter("p_channel_names", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.ChannelName).ToArray() },
        new NpgsqlParameter("p_extifs", NpgsqlDbType.Array | NpgsqlDbType.Text) { Value = streams.Select(s => s.ExtInf ?? "-1").ToArray() },
        new NpgsqlParameter("p_is_hidden", NpgsqlDbType.Array | NpgsqlDbType.Boolean) { Value = streams.Select(s => s.IsHidden).ToArray() },
        new NpgsqlParameter("p_tvg_names", NpgsqlDbType.Array | NpgsqlDbType.Citext) { Value = streams.Select(s => s.TVGName).ToArray() },

        // Create parameters for scalar values
        new NpgsqlParameter("p_m3u_file_id", NpgsqlDbType.Integer) { Value = m3uFileId },
        new NpgsqlParameter("p_m3u_file_name", NpgsqlDbType.Citext) { Value = m3uFileName },
        new NpgsqlParameter("p_stream_group_id", NpgsqlDbType.Integer) { Value = streamGroupId },
        new NpgsqlParameter("p_create_channels", NpgsqlDbType.Boolean) { Value = createChannels },
        new NpgsqlParameter("p_return_results", NpgsqlDbType.Boolean) { Value = returnResults }
    ];
#pragma warning restore RCS1130 // Bitwise operation on enum without Flags attribute

        return parameters;
    }

    private static bool ShouldUpdate(M3UFile m3uFile, List<string> VODTags)
    {
        if (string.IsNullOrEmpty(m3uFile.DirectoryLocation))
        {
            m3uFile.DirectoryLocation = FileDefinitions.M3U.DirectoryLocation;
            return true;
        }
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